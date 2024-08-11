using AutoMapper;
using Globe.Audit.Api.Database;
using Globe.Audit.Api.Events;
using Globe.Audit.Api.Models;
using Globe.Audit.Api.Services;
using Globe.Audit.Api.Services.Impl;
using Globe.Domain.Core.Data;
using Globe.EventBus.RabbitMQ.Config;
using Globe.EventBus.RabbitMQ.Receiver;
using Globe.EventBus.RabbitMQ.Receiver.Impl;
using Globe.EventBus.RabbitMQ.Sender;
using Globe.EventBus.RabbitMQ.Sender.Impl;
using Globe.Shared.Constants;
using Globe.Shared.Helpers;
using Globe.Shared.MiddlewareExtensions;
using Globe.Shared.Models;
using Globe.Shared.Options;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DataEncryption;
using Microsoft.EntityFrameworkCore.DataEncryption.Providers;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;
using System.Reflection;

namespace Globe.Audit.Api
{
    /// <summary>
    /// The startup.
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
        /// Gets the configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        /// <summary>
        /// Configures the services.
        /// </summary>
        /// <param name="services">The services.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            // Configure auto mapper
            ConfigureAutoMapper(services);

            services.AddSerilog((context, loggerConfiguration) =>
            {
                // read logger's config and set logger...
                LoggerHelper loggerHelper = new();
                loggerHelper.Configure(loggerConfiguration, Configuration);
            });

            // Adding MVC API controllers to services
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });

            string auditQueueName = Configuration.GetValue<string>("AuditQueueName");

            if (!string.IsNullOrWhiteSpace(auditQueueName))
                //get queue names from configuration
                RabbitMqQueuesConstants.AuditQueueName = Configuration.GetValue<string>("AuditQueueName");


            //Get Sql connection string
            string connectionString = GetSqlConnectionString();

            // Configures the encryption provider.
            ConfigureEncryptionProvider(services);

            // Create Logger Factory
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());

            // Configure common foundation db context
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                if (_isDevelopment) options.UseLoggerFactory(loggerFactory);
                options.UseSqlServer(GetSqlConnectionString("DBConnections:CommonDB"), x => x.MigrationsAssembly("Globe.Domain.Core"));

                if (_isDevelopment)
                {
                    options.ConfigureWarnings(warnings =>
                                              warnings.Throw(RelationalEventId.MultipleCollectionIncludeWarning));

                    options.ConfigureWarnings(warnings =>
                    {
                        warnings.Throw(CoreEventId.FirstWithoutOrderByAndFilterWarning);
                    });
                }
            });

            ConfigureEventBus(services);
            ConfigureSwagger(services);
            services.AddDbContext<AuditDbContext>(options =>
                    options.UseSqlServer(connectionString));

            // Setting up Http Context Accessor to be used to get username from header
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddScoped<IAuditService, AuditService>();

            // Get service information e.g., service name
            services.Configure<ServiceInformation>(Configuration.GetSection(nameof(ServiceInformation)));

            // Add Cors support
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            });

            //Configure masking and debug degrade logging settings
            LoggingSettingsModel loggingSettings = new LoggingSettingsModel();
            Configuration.GetSection("LoggingSettings").Bind(loggingSettings);
            services.AddSingleton(loggingSettings);

            services.AddScoped<LogMaskingHelper>();
            services.AddScoped<LogEntryDegradeHelper>();
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
        /// Gets the sql connection string.
        /// </summary>
        /// <param name="sectionName">The section name.</param>
        /// <returns>A string.</returns>
        private string GetSqlConnectionString(string sectionName)
        {
            SqlConnectionConfiguration config = Configuration.GetSection(sectionName)
                                                                .Get<SqlConnectionConfiguration>();
            return SqlConnectionHelper.ToConnectionString(config);
        }

        private string GetSqlConnectionString()
        {
            SqlConnectionConfiguration config = Configuration.GetSection($"DBConnections:LocalDB")
                                                                .Get<SqlConnectionConfiguration>();
            return SqlConnectionHelper.ToConnectionString(config);
        }

        /// <summary>
        /// Configures the event bus.
        /// </summary>
        /// <param name="services">The services.</param>
        private void ConfigureEventBus(IServiceCollection services)
        {
            services.AddOptions();
            services.Configure<RabbitMqConfiguration>(Configuration.GetSection("RabbitMq"));
            services.AddTransient<IEventSender, EventSender>();
            services.AddTransient<IQueueHandlerMappingFactory, QueueHandlerMappingFactory>();
            services.AddHostedService<EventListener>();
        }

        /// <summary>
        /// Configures the auto mapper.
        /// </summary>
        /// <param name="services">The services collection.</param>
        private static void ConfigureAutoMapper(IServiceCollection services)
        {
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);
        }

        /// <summary>
        /// Configures the swagger.
        /// </summary>
        /// <param name="services">The services collection.</param>
        private void ConfigureSwagger(IServiceCollection services)
        {
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(swagger =>
            {
                swagger.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Globe Foundation - Audit Service",
                    Description = "Globe Foundation - Audit Service",
                });
            });
        }

        /// <summary>
        /// Configures the.
        /// </summary>
        /// <param name="app">The app.</param>
        /// <param name="env">The env.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="logger">The logger</param>
        /// <param name="context">The database context</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, ILogger<Startup> logger, AuditDbContext context)
        {
            if (Configuration.GetValue<bool>("LogEachHttpRequest")) app.UseHttpLogging();

            if (!string.IsNullOrWhiteSpace(Configuration[AppLoggingConfigurationKeys.Enabled]) && Configuration[AppLoggingConfigurationKeys.Enabled].ToLower() == "true")
                app.UseLoggerMiddleware();   //<--- THIS IS A CUSTOM LOGGING MIDDLEWARE BASED ON SERILOG'S MIDDLEWARE

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
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Globe Foundation - Audit Service");
            });

            app.UseRouting();

            // Cors Policy
            app.UseCors();

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
