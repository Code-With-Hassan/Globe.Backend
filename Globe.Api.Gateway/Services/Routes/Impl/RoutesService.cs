using Microsoft.Extensions.Options;
using Ocelot.Configuration.File;

namespace Globe.Api.Gateway.Services.Routes.Impl
{
    /// <summary>
    /// The routes service.
    /// </summary>
    public class RoutesService : IRoutesService
    {
        private readonly List<FileRoute> _routes;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoutesService"/> class.
        /// </summary>
        public RoutesService(IOptions<List<FileRoute>> options)
        {
            _routes = options.Value;
        }

        /// <summary>
        /// Get all services base urls.
        /// </summary>
        /// <returns>Routes list.</returns>
        public List<string> GetAll()
        {
            var list = new List<string>();

            _routes.ForEach(route => route.DownstreamHostAndPorts.ForEach(host =>
                            list.Add(BuildUrl(route.DownstreamScheme, host.Host, host.Port))));

            list = list.Distinct().ToList();
            list.Sort();

            return list;
        }

        /// <summary>
        /// Build Url of service
        /// </summary>
        /// <param name="scheme">Scheme used by the service i.e., http</param>
        /// <param name="host">Host address of the service</param>
        /// <param name="port">Port number at which service is listening</param>
        /// <returns></returns>
        public static string BuildUrl(string scheme, string host, int port)
        {
            scheme = scheme == "ws" ? "http" : scheme == "wss" ? "https" : scheme;
            return $"{scheme}://{host}:{port}";
        }
    }
}
