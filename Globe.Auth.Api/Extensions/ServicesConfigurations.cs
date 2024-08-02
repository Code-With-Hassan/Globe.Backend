using Globe.Auth.Service.Data;
using Globe.Auth.Service.Services.AuthService;
using Globe.Auth.Service.Services.AuthService.Impl;
using Globe.Auth.Service.Services.RoleService.Impl;
using Globe.Auth.Service.Services.RoleService;
using Globe.Shared.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Globe.Auth.Service.Services.UserService;
using Globe.Auth.Service.Services.UserService.Impl;
using Globe.Auth.Service.Services.UserRegistrationService;
using Globe.Auth.Service.Services.UserRegistrationService.Impl;

namespace Globe.Auth.Api.Extensions
{
    /// <summary>
    /// Static class containing extension methods for configuring services.
    /// </summary>
    public static class ServicesConfigurations
    {
        /// <summary>
        /// Configures all necessary services for the application.
        /// </summary>
        /// <param name="builder">An instance of WebApplicationBuilder used to configure services.</param>
        public static void ConfigureServices(this WebApplicationBuilder builder)
        {
            // Configures Entity Framework services, including the database context and Identity.
            builder.Services.ConfigureEntityFramework(builder.Configuration);

            // Configures additional business services and extensions.
            builder.Services.ConfigureBusinessExtension();

            // Configures JWT authentication services.
            builder.Services.ConfigureJWTAuthentication(builder.Configuration);

            // Adds MVC controllers to the service collection.
            builder.Services.AddControllers();

            // Configures Swagger services for API documentation.
            builder.Services.ConfigureSwaggerService();
        }

        /// <summary>
        /// Configures Entity Framework services, including the database context and Identity.
        /// </summary>
        /// <param name="services">An IServiceCollection for registering services.</param>
        /// <param name="configuration">Configuration manager to access configuration settings.</param>
        public static void ConfigureEntityFramework(this IServiceCollection services, ConfigurationManager configuration)
        {
            // Configures the database context to use SQL Server with the specified connection string.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Adds Identity services with Entity Framework stores and default token providers.
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
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
        public static void ConfigureJWTAuthentication(this IServiceCollection services, ConfigurationManager configuration)
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
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Globe API", Version = "v1" });

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
    }
}
