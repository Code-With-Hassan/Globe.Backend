using System.Collections.Generic;

namespace Globe.Api.Gateway.Services.Authorization.Impl
{
    /// <summary>
    /// The authorize route settings.
    /// </summary>
    public class AuthorizeRouteSettings
    {
        /// <summary>
        /// Gets or sets the controllers to screen mappings.
        /// </summary>
        public List<ControllersToScreenMapping> ControllersToScreenMappings { get; set; }

        /// <summary>
        /// Gets or sets the controllers to application mappings.
        /// </summary>
        public List<ControllersToApplicationMapping> ControllersToApplicationMappings { get; set; }
    }

    /// <summary>
    /// The controllers to screen mapping.
    /// </summary>
    public class ControllersToScreenMapping
    {
        /// <summary>
        /// Gets or sets the screen name.
        /// </summary>
        public string ScreenName { get; set; }

        /// <summary>
        /// Gets or sets the admin controllers.
        /// User could send GET, POST, PUT AND DELETE requests to these controllers.
        /// </summary>
        public List<string> AdminControllers { get; set; }

        /// <summary>
        /// Gets or sets the readonly controllers.
        /// User could only send GET request to these controllers.
        /// </summary>
        public List<string> ReadOnlyControllers { get; set; }
    }

    /// <summary>
    /// The controllers to application mapping.
    /// </summary>
    public class ControllersToApplicationMapping
    {
        /// <summary>
        /// Gets or sets the application name.
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// Gets or sets the admin controllers.
        /// User could send GET, POST, PUT AND DELETE requests to these controllers.
        /// </summary>
        public List<string> AdminControllers { get; set; }

        /// <summary>
        /// Gets or sets the readonly controllers.
        /// User could only send GET request to these controllers.
        /// </summary>
        public List<string> ReadOnlyControllers { get; set; }
    }
}