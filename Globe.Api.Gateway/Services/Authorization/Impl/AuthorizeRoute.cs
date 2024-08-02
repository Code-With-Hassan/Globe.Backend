using Globe.Api.Gateway.Services.Authorization;
using Microsoft.Extensions.Options;

namespace Globe.Api.Gateway.Services.Authorization.Impl
{
    /// <summary>
    /// The route authorization.
    /// </summary>
    public class AuthorizeRoute : IAuthorizeRoute
    {
        private readonly AuthorizeRouteSettings _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizeRoute"/> class.
        /// </summary>
        /// <param name="options">The cached privileges settings.</param>
        public AuthorizeRoute(IOptions<AuthorizeRouteSettings> options) => _options = options.Value;

        /// <summary>
        /// Checks if request is allowed
        /// </summary>
        /// <param name="allowedApplicationsList"></param>
        /// <param name="allowedScreensList">The List of allowed screens.</param>
        /// <param name="path">The path.</param>
        /// <param name="method">The method.</param>
        /// <returns>A Task.</returns>
        public async Task<bool> IsAllowedAsync(List<string> allowedApplicationsList, List<string> allowedScreensList, string path, string method)
        {
            return await Task.Run(() =>
            {
                // Check privileges against requested path and method for the user here
                var parts = path.Trim('/').Split('/');

                var controller = parts.Length > 3 ? parts[3] : parts.Length > 2 ? parts[2] : string.Empty;
                if (string.IsNullOrEmpty(controller)) return false;

                var mappedScreensList = new List<string>();
                var mappedApplicationsList = new List<string>();

                mappedScreensList.AddRange(GetMappedAdminScreens(controller));
                mappedApplicationsList.AddRange(GetMappedAdminApplications(controller));

                if (method.ToUpper() == "GET")
                {
                    mappedScreensList.AddRange(GetMappedReadOnlyScreens(controller));
                    mappedApplicationsList.AddRange(GetMappedReadOnlyApplications(controller));
                }

                return IsMappedScreenAllowed(mappedScreensList, allowedScreensList) || IsMappedApplicationAllowed(mappedApplicationsList, allowedApplicationsList);
            });
        }

        /// <summary>
        /// Are the mapped screens allowed.
        /// </summary>
        /// <param name="mappedScreensList">The mapped screens list.</param>
        /// <param name="allowedScreensList">The allowed screens list.</param>
        /// <returns>A bool.</returns>
        private static bool IsMappedScreenAllowed(List<string> mappedScreensList, List<string> allowedScreensList)
        {
            return mappedScreensList != null && allowedScreensList != null &&
                    mappedScreensList.Count > 0 && allowedScreensList.Count > 0 &&
                    mappedScreensList.Any(a => allowedScreensList.Any(b => string.Compare(a, b, true) == 0));
        }

        /// <summary>
        /// Gets the list of mapped read only screens.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <returns>A list of string.</returns>
        private IEnumerable<string> GetMappedReadOnlyScreens(string controller)
        {
            return _options.ControllersToScreenMappings
                    .FindAll(x => x.ReadOnlyControllers != null &&
                                    x.ReadOnlyControllers.Count > 0 &&
                                    x.ReadOnlyControllers.Exists(y => string.Compare(controller, y, true) == 0))
                    .Select(x => x.ScreenName).ToList();
        }

        /// <summary>
        /// Gets the list of mapped admin screens.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <returns>A list of string.</returns>
        private IEnumerable<string> GetMappedAdminScreens(string controller)
        {
            return _options.ControllersToScreenMappings
                    .FindAll(x => x.AdminControllers != null &&
                                    x.AdminControllers.Count > 0 &&
                                    x.AdminControllers.Exists(y => string.Compare(controller, y, true) == 0))
                    .Select(x => x.ScreenName).ToList();
        }

        /// <summary>
        /// Are the mapped application allowed.
        /// </summary>
        /// <param name="mappedApplicationsList">The mapped application list.</param>
        /// <param name="allowedApplicationsList">The allowed application list.</param>
        /// <returns>A bool.</returns>
        private static bool IsMappedApplicationAllowed(List<string> mappedApplicationsList, List<string> allowedApplicationsList)
        {
            return mappedApplicationsList != null && allowedApplicationsList != null &&
                    mappedApplicationsList.Count > 0 && allowedApplicationsList.Count > 0 &&
                    mappedApplicationsList.Any(a => allowedApplicationsList.Any(b => string.Compare(a, b, true) == 0));
        }

        /// <summary>
        /// Gets the list of mapped read only application.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <returns>A list of string.</returns>
        private IEnumerable<string> GetMappedReadOnlyApplications(string controller)
        {
            return _options.ControllersToApplicationMappings
                    .FindAll(x => x.ReadOnlyControllers != null &&
                                    x.ReadOnlyControllers.Count > 0 &&
                                    x.ReadOnlyControllers.Exists(y => string.Compare(controller, y, true) == 0))
                    .Select(x => x.ApplicationName).ToList();
        }

        /// <summary>
        /// Gets the list of mapped admin application.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <returns>A list of string.</returns>
        private IEnumerable<string> GetMappedAdminApplications(string controller)
        {
            return _options.ControllersToApplicationMappings
                    .FindAll(x => x.AdminControllers != null &&
                                    x.AdminControllers.Count > 0 &&
                                    x.AdminControllers.Exists(y => string.Compare(controller, y, true) == 0))
                    .Select(x => x.ApplicationName).ToList();
        }
    }
}
