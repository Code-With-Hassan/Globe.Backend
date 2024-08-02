using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Globe.Api.Gateway.Middleware
{
    /// <summary>
    /// Custom headers middleware.
    /// Adds custom headers to response.
    /// </summary>
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityHeadersMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next.</param>
        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Invokes asynchronously.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>A Task.</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Content-Type-Options
            context.Response.Headers.Add("X-Content-Type-Options", "nosniff");

            // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-XSS-Protection
            context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");

            // https://developer.mozilla.org/en-US/docs/Web/HTTP/CSP
            context.Response.Headers.Add("Content-Security-Policy", "self");

            // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Frame-Options
            context.Response.Headers.Add("X-Frame-Options", "DENY");

            // Call next middleware
            await _next(context);
        }
    }
}
