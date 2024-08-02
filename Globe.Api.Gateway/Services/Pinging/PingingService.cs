
using Globe.Api.Gateway.Services.Routes;

namespace Globe.Api.Gateway.Services.Pinging
{
    /// <summary>
    /// The pinging service.
    /// </summary>
    public class PingingService : BackgroundService
    {
        private readonly List<string> _routesList;
        private readonly HttpClient _httpClient;
        private readonly int _pingDelayInMilliseconds;
        private readonly ILogger<PingingService> _logger;
        private static readonly List<string> _ignoreRoutesList = new();
        private static readonly object _lock = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="PingingService"/> class.
        /// </summary>
        /// <param name="provider">The service provider.</param>
        /// <param name="logger">The logger for pinging service.</param>
        public PingingService(IServiceProvider provider, ILogger<PingingService> logger)
        {
            _logger = logger;
            _logger.LogInformation("Pinging service initializing...");

            var scope = provider.CreateScope();
            var routesService = scope.ServiceProvider.GetService<IRoutesService>();
            var config = scope.ServiceProvider.GetService<IConfiguration>();

            int pingDelayInSeconds = config.GetValue<int>(nameof(pingDelayInSeconds));

            _pingDelayInMilliseconds = pingDelayInSeconds * 1000;
            _httpClient = scope.ServiceProvider.GetService<HttpClient>();
            _routesList = routesService.GetAll();
            _routesList = _routesList.Select(x => x.Trim('/') + "/api/v1/monitoring/ping").ToList();

            _logger.LogInformation("...Pinging service initialized successfully!");
        }

        /// <summary>
        /// Executes the logic async.
        /// </summary>
        /// <param name="stoppingToken">The stopping token.</param>
        /// <returns>A Task.</returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Pinging service execution started...");

            while (!stoppingToken.IsCancellationRequested)
            {
                // Wait for few second(s)
                await Task.Delay(_pingDelayInMilliseconds, stoppingToken);

                // Ping all routes
                await PingServicesAsync();
            }

            _logger.LogInformation("...Pinging service execution stopped!");
        }

        /// <summary>
        /// Pings the services async.
        /// </summary>
        /// <returns>A Task.</returns>
        private async Task PingServicesAsync()
        {
            try
            {
                foreach (var apiRoute in _routesList)
                {
                    await PingApi(apiRoute);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Pinging service error: {Message}", ex.Message);
            }
        }

        /// <summary>
        /// Pings the api.
        /// </summary>
        /// <param name="pingRoute">The api ping route.</param>
        /// <returns>A Task.</returns>
        private async Task PingApi(string pingRoute)
        {
            try
            {
                _ = await _httpClient.GetAsync(pingRoute);
                RemoveFromIgnoreRoutesList(pingRoute);
            }
            catch
            {
                if (!_ignoreRoutesList.Contains(pingRoute))
                    AddToIgnoreRoutesList(pingRoute);
            }
        }

        /// <summary>
        /// Add to ignore routes list
        /// </summary>
        /// <param name="url">url to add</param>
        public static void AddToIgnoreRoutesList(string url)
        {
            lock (_lock)
            {
                _ignoreRoutesList.Add(url);
            }
        }

        /// <summary>
        /// Remove from ignore routes list
        /// </summary>
        /// <param name="url">url to remove</param>
        public static void RemoveFromIgnoreRoutesList(string url)
        {
            lock (_lock)
            {
                _ignoreRoutesList.Remove(url);
            }
        }

        /// <summary>
        /// Checks if service is up and running
        /// </summary>
        /// <param name="url">Url of the service</param>
        /// <returns>returns true if last ping was successfull or else return false</returns>
        public static bool IsServiceUp(string url)
        {
            lock (_lock)
            {
                return !_ignoreRoutesList.Exists(x => x.Length >= url.Length && string.Compare(x[..url.Length], url, true) == 0);
            }
        }
    }
}