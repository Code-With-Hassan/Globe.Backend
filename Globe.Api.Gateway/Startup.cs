using Globe.Api.Gateway.Extensions;
using Globe.Api.Gateway.Middleware;
using Globe.Api.Gateway.Services.Authorization;
using Globe.Api.Gateway.Services.Authorization.Impl;
using Globe.Api.Gateway.Services.Routes;
using Globe.Api.Gateway.Services.Routes.Impl;
using Globe.Shared.Constants;
using Globe.Shared.Helpers;
using Globe.Shared.MiddlewareExtensions;
using Globe.Shared.Models;
using Globe.Shared.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using Ocelot;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;
using Ocelot.ServiceDiscovery.Providers;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Text;
using IPNetwork = Microsoft.AspNetCore.HttpOverrides.IPNetwork;

namespace Globe.Api.Gateway
{
    public class Startup
    {
        private readonly string _myAllowSpecificOrigins = "MyAllowSpecificOrigins";
        private bool _forwardOriginatorIpAddress;
        private List<IPNetwork> _knownNetworks;
        private List<IPAddress> _knownProxies;

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _forwardOriginatorIpAddress = false;
            _knownNetworks = new List<IPNetwork>();
            _knownProxies = new List<IPAddress>();
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Configures the services for the web api project
        /// </summary>
        /// <param name="services">The services injected</param>
        public void ConfigureServices(IServiceCollection services)
        {
            // Adding MVC API controllers to services
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });

            // Load routes from configuration.
            services.Configure<List<Ocelot.Configuration.File.FileRoute>>(Configuration.GetSection("Routes"));

            // Configure routes service
            services.AddScoped<IRoutesService, RoutesService>();

            //Func<IServiceProvider, DownstreamRoute, IServiceDiscoveryProvider, CustomLoadBalancer> loadBalancerFactoryFunc = (serviceProvider, Route, serviceDiscoveryProvider) => new CustomLoadBalancer(serviceDiscoveryProvider.Get);

            // Addign Ocelt web api capabilities, and add a dictionary based cache
            services.AddOcelot()
                    .AddPolly();
                    //.AddCacheManager(x => x.WithDictionaryHandle())
                    //.AddCustomLoadBalancer(loadBalancerFactoryFunc);

            // Add authentication
            var key = Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            // Load routes from configuration.
            services.Configure<List<Ocelot.Configuration.File.FileRoute>>(Configuration.GetSection("Routes"));

            // Ocelot pipeline configuration service settings.
            services.Configure<AuthMiddlewareSettings>(Configuration.GetSection(nameof(AuthMiddlewareSettings)));

            // Ocelot pipeline configuration service.
            services.AddSingleton<RequestValidatorConfiguration>();

            // Get origins list from configuration
            string[] AllowedOrigins = Configuration.GetSection(nameof(AllowedOrigins)).Get<string[]>();

            // Add Cors support
            services.AddCors(options =>
            {
                options.AddPolicy(name: _myAllowSpecificOrigins,
                builder =>
                {
                    //Need to specify origin for SignalR cors
                    builder.WithOrigins(AllowedOrigins)
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                });
            });

            // Configure known networks and proxies
            ConfigureKnownNetworksAndProxies();

            // Add http client
            services.AddHttpClient();

            // Add Authorize route settings
            services.Configure<AuthorizeRouteSettings>(Configuration.GetSection(nameof(AuthorizeRouteSettings)));

            // Add Authorize route service
            services.AddSingleton<IAuthorizeRoute, AuthorizeRoute>();

            // Get service information e.g., service name
            services.Configure<ServiceInformation>(Configuration.GetSection(nameof(ServiceInformation)));

            // Configure SignalR
            // services.AddSignalR();

            //Configure masking and debug degrade logging settings
            LoggingSettingsModel loggingSettings = new LoggingSettingsModel();
            Configuration.GetSection("LoggingSettings").Bind(loggingSettings);
            services.AddSingleton(loggingSettings);

            services.AddScoped<LogMaskingHelper>();
            services.AddScoped<LogEntryDegradeHelper>();
        }

        /// <summary>
        /// Configures whether the application will forward 
        /// the originator IP address to the backend services,
        /// as well as the known networks and proxies for which
        /// the feature will be enabled.s
        /// </summary>
        private void ConfigureKnownNetworksAndProxies()
        {
            _forwardOriginatorIpAddress = Configuration.GetValue<bool>("ForwardOriginatorIpAddress:Enabled");
            if (_forwardOriginatorIpAddress)
            {
                List<string> knownNetworksAndProxies = new List<string>();
                Configuration.GetSection("ForwardOriginatorIpAddress:KnownNetworksAndProxies")
                    .Bind(knownNetworksAndProxies);

                foreach (var knownNetworkOrProxy in knownNetworksAndProxies)
                {
                    if (IPAddress.TryParse(knownNetworkOrProxy, out var ipAddress))
                    {
                        if (!_knownProxies.Contains(ipAddress))
                            _knownProxies.Add(ipAddress);
                    }
                    else if (knownNetworkOrProxy.Split('/').Count() == 2)
                    {
                        string subnet = knownNetworkOrProxy.Split('/')[0];
                        string mask = knownNetworkOrProxy.Split('/')[1];
                        if (IPAddress.TryParse(subnet, out var subnetIp))
                        {
                            if (Int32.TryParse(mask, out var prefixLength) &&
                                prefixLength <= 32)
                            {
                                var ipNetwork = new IPNetwork(subnetIp, prefixLength);
                                if (!_knownNetworks.Contains(ipNetwork))
                                    _knownNetworks.Add(ipNetwork);
                            }
                            else
                            {
                                Debug.WriteLine($"Invalid prefix length: {prefixLength}.");
                                Console.WriteLine($"Invalid prefix length: {prefixLength}.");
                            }
                        }
                        else
                        {
                            Debug.WriteLine($"Invalid subnet address: {subnet}.");
                            Console.WriteLine($"Invalid subnet address: {subnet}.");
                        }
                    }
                    else
                    {
                        Debug.WriteLine($"Invalid network or proxy address: {knownNetworkOrProxy}.");
                        Console.WriteLine($"Invalid network or proxy address: {knownNetworkOrProxy}.");
                    }
                }
            }
        }

        /// <summary>
        /// Configures the API.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="env">The web host environment.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="logger">The startup logger.</param>
        /// <param name="services">The configured services from service provider.</param>
        public async void Configure(IApplicationBuilder app,
                                    IWebHostEnvironment env,
                                    ILoggerFactory loggerFactory,
                                    ILogger<Startup> logger,
                                    IServiceProvider services)
        {

            logger.LogInformation("Service \"{ServiceName}\" Started Successfully.", Configuration["ServiceInformation:ServiceName"]);
            logger.LogInformation("ServicePath: {CurrentPath}", Program.CurrentPath);
            logger.LogInformation("Version: {Version}", Assembly.GetExecutingAssembly().GetName().Version);
            logger.LogInformation("Profile: {ProcessName}", System.Diagnostics.Process.GetCurrentProcess().ProcessName);

            if (!string.IsNullOrWhiteSpace(Configuration[AppLoggingConfigurationKeys.Enabled]) && Configuration[AppLoggingConfigurationKeys.Enabled].ToLower() == "true")
                app.UseLoggerMiddleware();   //<--- THIS IS A CUSTOM LOGGING MIDDLEWARE BASED ON THE SERILOG'S MIDDLEWARE

            if (Configuration.GetValue<bool>("UseHttpsRedirection"))
                app.UseHttpsRedirection();

            app.UseRouting();

            #region Forward originator IP address

            if (_forwardOriginatorIpAddress)
            {
                app.Use(async (context, next) =>
                {
                    if (!context.Request.Headers.ContainsKey("X-Forwarded-For"))
                    {
                        context.Request.Headers.Add("X-Forwarded-For", context.Connection.RemoteIpAddress.ToString());
                    }

                    await next();
                });

                ForwardedHeadersOptions forwardedHeadersOptions = new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
                    ForwardLimit = null,
                    RequireHeaderSymmetry = false,
                    ForwardedForHeaderName = "X-Forwarded-For"
                };

                foreach (var knownNetwork in _knownNetworks)
                    forwardedHeadersOptions.KnownNetworks.Add(knownNetwork);
                foreach (var knownProxy in _knownProxies)
                    forwardedHeadersOptions.KnownProxies.Add(knownProxy);

                app.UseForwardedHeaders(forwardedHeadersOptions);
            }
            #endregion

            // Cors Policy
            app.UseCors(_myAllowSpecificOrigins);

            app.UseAuthentication();
            app.UseAuthorization();

            // Use corelation id middleware
            app.UseCorelationId();

            // Use endpoints
            app.UseEndpoints(endpoints => endpoints.MapControllers());

            // Use Security headers middleware
            app.UseSecurityHeaders();

            //get service here
            var ocelotPipelineConfig = services.GetRequiredService<RequestValidatorConfiguration>();

            // App ocelot capabilities
            await app.UseOcelot(ocelotPipelineConfig);
        }
    }
}
