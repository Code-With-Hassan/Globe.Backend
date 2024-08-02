using Globe.Api.Gateway.Middleware;

namespace Globe.Api.Gateway.Extensions
{
    /// <summary>
    /// The total care middleware extensions.
    /// </summary>
    public static class GatewayExtensions
    {
        /// <summary>
        /// Uses the corelation id.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>An IApplicationBuilder.</returns>
        public static IApplicationBuilder UseCorelationId(this IApplicationBuilder builder)
            => builder.UseMiddleware<CorelationIdMiddleware>();

        /// <summary>
        /// Use security headers middleware.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>An IApplicationBuilder.</returns>
        public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder builder)
            => builder.UseMiddleware<SecurityHeadersMiddleware>();
    }
}