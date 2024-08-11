using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Globe.Account.Service.Services.RoleService;
using Globe.Account.Service.Services.UserRegistrationService;
using Globe.Account.Service.Services.UserService;
using Globe.Account.Service.Services.AuthService;
using Globe.Account.Service.Services.UserService.Impl;
using Globe.Account.Service.Services.AuthService.Impl;
using Globe.Account.Service.Services.UserRegistrationService.Impl;
using Globe.Account.Service.Services.RoleService.Impl;
using Globe.Domain.Core.Data;
using Microsoft.Extensions.Configuration;
using Globe.EventBus.RabbitMQ.Config;
using Globe.EventBus.RabbitMQ.Sender;
using Globe.EventBus.RabbitMQ.Sender.Impl;
using Globe.Shared.Entities;
using Globe.Account.Service.Services;

namespace Globe.Account.Api.Extensions
{
    /// <summary>
    /// Static class containing extension methods for configuring services.
    /// </summary>
    public static class ServicesConfigurations
    {
        /// <summary>
        /// Configures all necessary services for the application.
        /// </summary>
        /// <param name="builder">An instance of IServiceCollection used to configure services.</param>
        public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Configures Entity Framework services, including the database context and Identity.
            services.ConfigureEntityFramework(configuration);

            // Configures additional business services and extensions.
            services.ConfigureBusinessExtension();

            // Configure the Event Bus service (RabbitMQ)
            services.ConfigureEventBus(configuration);

            // Configures JWT authentication services.
            services.ConfigureJWTAuthentication(configuration);

            // Adds MVC controllers to the service collection.
            services.AddControllers();

            // Configures Swagger services for API documentation.
            services.ConfigureSwaggerService();
        }

        /// <summary>
        /// Configures Entity Framework services, including the database context and Identity.
        /// </summary>
        /// <param name="services">An IServiceCollection for registering services.</param>
        /// <param name="configuration">Configuration manager to access configuration settings.</param>
        public static void ConfigureEntityFramework(this IServiceCollection services, IConfiguration configuration)
        {
            // Create Logger Factory
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());

            // Configures the database context to use SQL Server with the specified connection string.
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                options.UseLoggerFactory(loggerFactory);

            });

            // Adds Identity services with Entity Framework stores and default token providers.
            /*services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();*/

            // Adding MS Identity and Role to the services
            services.AddIdentity<UserAuthEntity, IdentityRole>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                //Lock user
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(1);
                options.Lockout.MaxFailedAccessAttempts = 2;// configuration.GetValue<int>("PasswordAuthentication:WrongAttempt");

                // User settings.
                options.User.RequireUniqueEmail = true;

                // Signin options.
                options.SignIn.RequireConfirmedAccount = true;

                // Token provider.
                options.Tokens.PasswordResetTokenProvider = "Default";
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                //.AddErrorDescriber<CustomIdentityErrorDescriber>()
                .AddDefaultTokenProviders()
                .AddTokenProvider<DefaultDataProtectorTokenProvider<UserAuthEntity>>("Default");


        }

        /// <summary>
        /// Configures additional business services.
        /// </summary>
        /// <param name="services">An IServiceCollection for registering services.</param>
        public static void ConfigureBusinessExtension(this IServiceCollection services)
        {
            // Registers the authentication service with a scoped lifetime.
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRegistrationService, UserRegistrationService>();

            services.AddLogging();
        }

        /// <summary>
        /// Configures JWT authentication services.
        /// </summary>
        /// <param name="services">An IServiceCollection for registering services.</param>
        /// <param name="configuration">Configuration manager to access configuration settings.</param>
        public static void ConfigureJWTAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            // Retrieves JWT settings from the configuration.
            var jwtSettings = configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

            // Configures authentication with JWT bearer tokens.
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                // Sets token validation parameters including issuer, audience, and signing key.
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });
        }

        /// <summary>
        /// Configures Swagger services for API documentation.
        /// </summary>
        /// <param name="services">An IServiceCollection for registering services.</param>
        public static void ConfigureSwaggerService(this IServiceCollection services)
        {
            // Configures Swagger generation with basic settings.
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Globe - Accounts Service",
                    Description = "Globe Foundation - Accounts Service",
                });

                // Adds JWT authentication to the Swagger UI.
                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "JWT Authentication",
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };
                options.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { securityScheme, new string[] { } }
                });
            });
            
        }

        /// <summary>
        /// Configures the event bus.
        /// </summary>
        /// <param name="services">The services required to configure.</param>
        private static void ConfigureEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();

            services.Configure<RabbitMqConfiguration>(configuration.GetSection("RabbitMq"));
            services.AddTransient<IEventSender, EventSender>();
        }
    }
}
