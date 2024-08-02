namespace Globe.Shared.Models
{
    public class LoggingSettingsModel
    {
        /// <summary>
        /// Enables/disables logging masking for sensitive info.
        /// </summary>
        public bool Enable { get; set; }
        /// <summary>
        /// The list of requests to be masked in the log file.
        /// </summary>
        public List<RequestsLogMaskingSettings> Requests { get; set; } = new List<RequestsLogMaskingSettings>();
        /// <summary>
        /// Contains the request URL prefixes that whose logs will be degraded to Debug.
        /// This prevents flooding the log file with useless entries.
        /// </summary>
        public List<string> DebugDegrade { get; set; } = new List<string>();
    }

    public class RequestsLogMaskingSettings
    {
        /// <summary>
        /// The request URL without the base URL
        /// (e.g. /api/users/login).
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// The list of properties to be masked in the log file.
        /// </summary>
        public List<PropertyLogMaskingSettings> Properties { get; set; } = new List<PropertyLogMaskingSettings>();
    }

    public class PropertyLogMaskingSettings
    {
        /// <summary>
        /// The name of the property to be masked.
        /// </summary>
        public string PropertyName { get; set; }
        /// <summary>
        /// Determines whether the property will be masked partially.
        /// If it is false, the property will be masked fully,
        /// without showing its actual length (as a string).
        /// </summary>
        public bool PartialMask { get; set; }
        /// <summary>
        /// The index of the string where the masking will start.
        /// </summary>
        public int StartIndex { get; set; }
        /// <summary>
        /// The index of the string where the masking will end.
        /// </summary>
        public int EndIndex { get; set; }
    }
}
