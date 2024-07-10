using Global.Shared.Constants;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Globe.Auth.Api.Middlewares
{
    public class CustomAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string[] _allowedPaths;
        private readonly IConfiguration _configuration;

        public CustomAuthMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;

            // Define allowed paths here
            _allowedPaths = new string[] { "/api/auth/login" };
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                // Check if path is allowed before token validation
                string currentPath = context.Request.Path.Value.ToLower();
                if (_allowedPaths.Any(path => path == currentPath))
                {
                    await _next(context);
                    return;
                }

                // Extract token from Authorization header
                string token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(' ').LastOrDefault();

                if (string.IsNullOrEmpty(token))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Unauthorized");
                    return;
                }

                // Read secret key from configuration
                var secretKey = _configuration[JWTSettingsContants.JWT_KEY];

                // Validate token using JWT validation library
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                SecurityToken validatedToken;

                try
                {
                    tokenHandler.ValidateToken(token, tokenValidationParameters, out validatedToken);
                }
                catch (Exception ex)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Invalid token");
                    return;
                }

                // Attach user information to context (if needed)
                var principal = tokenHandler.ReadJwtToken(token);
                context.Items["UserObj"] = principal;

                // Continue processing the request
                await _next(context);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            }
        }
    }
}
