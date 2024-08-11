using Globe.Account.Api.Extensions;
using Globe.Shared.Constants;
using Globe.Shared.Helpers;
using Globe.Shared.MiddlewareExtensions;
using Globe.Shared.Models;
using Globe.Shared.Options;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.HttpLogging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;
using System.Reflection;
using Microsoft.EntityFrameworkCore.DataEncryption;
using Microsoft.EntityFrameworkCore.DataEncryption.Providers;
using AutoMapper;
using Globe.Account.Service.Models.Mapping;

namespace Globe.Account.Api
{
    /// <summary>
    /// The startup of the API project.
    /// </summary>
    public class Startup
    {
        private readonly bool _isDevelopment = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="env">The IWebHostEnvironment</param>
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _isDevelopment = env.IsDevelopment();
        }

        /// <summary>
        /// Gets the configuration object from appsettings.json.
        /// </summary>
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        /// <summary>
        /// Configures the services.
        /// </summary>
        /// <param name="services">The services to which we can call extension methods or inject IOC objects</param>
        public void ConfigureServices(IServiceCollection services)
        {
            // Adding MVC API controllers to services
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });

            // Setting up Http Context Accessor to be used to get username from header
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.ConfigureServices(Configuration);

            // Configures the encryption provider.
            ConfigureEncryptionProvider(services);

            // Get service information e.g., service name
            services.Configure<ServiceInformation>(Configuration.GetSection(nameof(ServiceInformation)));

            // Add Cors support
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            });

            services.AddHttpLogging(logging =>
            {
                logging.LoggingFields = HttpLoggingFields.All;
                logging.RequestHeaders.Add("sec-ch-ua");
                logging.ResponseHeaders.Add("MyResponseHeader");
                logging.MediaTypeOptions.AddText("application/javascript");
                logging.RequestBodyLogLimit = 4096;
                logging.ResponseBodyLogLimit = 4096;
                logging.CombineLogs = true;
            });


            //Configure masking and debug degrade logging settings
            LoggingSettingsModel loggingSettings = new LoggingSettingsModel();
            Configuration.GetSection("LoggingSettings").Bind(loggingSettings);
            services.AddSingleton(loggingSettings);

            services.AddSerilog((context, loggerConfiguration) =>
            {
                // read logger's config and set logger...
                LoggerHelper loggerHelper = new();
                loggerHelper.Configure(loggerConfiguration, Configuration);
            });

            services.AddScoped<LogMaskingHelper>();
            services.AddScoped<LogEntryDegradeHelper>();

            ConfigureAutoMapper(services);
        }

        /// <summary>
        /// Configures the auto mapper.
        /// </summary>
        /// <param name="services">The services collection.</param>
        private void ConfigureAutoMapper(IServiceCollection services)
        {
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);
        }

        /// <summary>
        /// Configures the encryption provider.
        /// </summary>
        /// <param name="services">The services.</param>
        private void ConfigureEncryptionProvider(IServiceCollection services)
        {
            string encryptionKey = EncryptionHelper.GetKey();
            IEncryptionProvider encryptionProvider = new AesProvider(Convert.FromBase64String(encryptionKey));
            services.AddSingleton(encryptionProvider);
        }

        /// <summary>
        /// Configures the application based on environment.
        /// </summary>
        /// <param name="app">The application builder object</param>
        /// <param name="env">The environment object.</param>
        /// <param name="logger">The logger.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            if (Configuration.GetValue<bool>("LogEachHttpRequest")) app.UseHttpLogging();

            if (!string.IsNullOrWhiteSpace(Configuration[AppLoggingConfigurationKeys.Enabled]) && Configuration[AppLoggingConfigurationKeys.Enabled].ToLower() == "true")
                app.UseMiddleware<LoggingMiddleware>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(a => a.Run(async context =>
                {
                    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                    var exception = exceptionHandlerPathFeature.Error;

                    // Log Exception
                    logger.LogError(exception.Message, exception);

                    var result = JsonConvert.SerializeObject(new { error = exception.Message });
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(result);
                }));
            }

            if (Configuration.GetValue<bool>("UseHttpsRedirection"))
                app.UseHttpsRedirection();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger(c =>
            {
                c.SerializeAsV2 = true;
            });

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Globe Foundation - Account Service");
            });

            app.UseRouting();

            // Cors Policy
            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            logger.LogInformation("Service Started Successfully.");
            logger.LogInformation("ServicePath: {0}", AppContext.BaseDirectory);
            logger.LogInformation("Version: {0}", Assembly.GetExecutingAssembly().GetName().Version);
            logger.LogInformation("Profile: {0}", System.Diagnostics.Process.GetCurrentProcess().ProcessName);
        }
    }
}
