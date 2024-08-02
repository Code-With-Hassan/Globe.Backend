using Globe.Api.Gateway.Services.Authorization;
using Globe.Shared.Constants;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Ocelot.Middleware;
using System.IdentityModel.Tokens.Jwt;

namespace Globe.Api.Gateway.Middleware
{
    /// <summary>
    /// The request validator configuration.
    /// </summary>
    public class RequestValidatorConfiguration : OcelotPipelineConfiguration
    {
        static readonly JwtSecurityTokenHandler _handler = new();

        private readonly AuthMiddlewareSettings _options;
        private readonly IAuthorizeRoute _routeAuth;
        private readonly ILogger<RequestValidatorConfiguration> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestValidatorConfiguration"/> class.
        /// </summary>
        public RequestValidatorConfiguration(IOptions<AuthMiddlewareSettings> options,
                                                IAuthorizeRoute routeAuth,
                                                ILogger<RequestValidatorConfiguration> logger)
        {
            _options = options.Value;
            _routeAuth = routeAuth;
            _logger = logger;

            // Override ocelot gateway middlewares
            PreErrorResponderMiddleware = ValidateRequest;
        }

        /// <summary>
        /// Validates the request.
        /// </summary>
        /// <param name="ctx">The HttpContext.</param>
        /// <param name="next">The next middleware.</param>
        /// <returns>A Task.</returns>
        async Task ValidateRequest(HttpContext ctx, Func<Task> next)
        {
            // If no authorization header exists in request, let gateway handle request.
            if (!_options.Enabled ||
                !ctx.Request.Headers.ContainsKey(HeaderNames.Authorization) ||
                !ctx.Request.Path.HasValue)
            {
                await next.Invoke();
                return;
            }

            // Check if path is required to be checked or not
            // e.g., Metadata or Preferences are not required to be checked.
            var path = ctx.Request.Path.Value;
            var method = ctx.Request.Method;

            if (_options.ContainPath(path, method))
            {
                await next.Invoke();
                return;
            }

            // Proceed to check path
            // Extract token
            var token = ctx.Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", string.Empty);

            // Read token
            var jwtSecurityToken = _handler.ReadJwtToken(token);

            // Extract user name from claim
            var userName = jwtSecurityToken.Claims.First(claim => claim.Type == IAuthConstants.UserName).Value;

            // Extract privileges from claim
            var privilegesString = jwtSecurityToken.Claims.First(claim => claim.Type == IAuthConstants.Privileges).Value;

            // Read privileges
            var privilegesList = JsonConvert.DeserializeObject<List<string>>(privilegesString);

            // Extract applications from claim
            var applicationsString = jwtSecurityToken.Claims.First(claim => claim.Type == IAuthConstants.Applications).Value;

            // Read application
            var applicationsList = JsonConvert.DeserializeObject<List<string>>(applicationsString);

            if (await _routeAuth.IsAllowedAsync(applicationsList, privilegesList, path, method))
            {
                // Proceed to next middleware
                await next.Invoke();

                return;
            }

            _logger.LogCritical("UserName: {0} is UnAuthorized, Path: {1}, Method: {2}", userName, path, method);

            ctx.Response.StatusCode = 401; // Un-Authroized.
        }
    }
}
