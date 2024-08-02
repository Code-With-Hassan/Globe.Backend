namespace Globe.Shared.Options
{
    /// <summary>
    /// The service information model.
    /// </summary>
    public class ServiceInformation
    {
        /// <summary>
        /// Gets or sets the service name.
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets if health checks enabled or not.
        /// </summary>
        public bool HealthChecksEnabled { get; set; }

        /// <summary>
        /// Convert to string
        /// </summary>
        /// <returns>Return string</returns>
        public override string ToString()
        {
            return $"Service Name: {ServiceName}, Version: {Version}";
        }
    }
}
