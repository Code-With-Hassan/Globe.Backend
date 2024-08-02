namespace Globe.Api.Gateway.Middleware
{
    /// <summary>
    /// The corelation id middleware.
    /// Adds corelation id to header.
    /// </summary>
    public class CorelationIdMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Initializes a new instance of the <see cref="CorelationIdMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next.</param>
        public CorelationIdMiddleware(RequestDelegate next)
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
            // Add corelation id
            // context.Request.Headers.Add(IAuthConstants.CorelationId, Guid.NewGuid().ToString());

            // Call next middleware
            await _next(context);
        }
    }
}
