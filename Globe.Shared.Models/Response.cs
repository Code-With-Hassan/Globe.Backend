namespace Globe.Shared.Models
{
    /// <summary>
    /// The response class.
    /// Used by all controllers to build a response for the request.
    /// </summary>
    public class Response
    {
        /// <summary>
        /// The request status.
        /// </summary>
        public enum RequestStatus { Success = 0, Error = 1 }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        public RequestStatus Status { get; set; } = RequestStatus.Success;

        /// <summary>
        /// Gets or sets the payload.
        /// </summary>
        public Dictionary<string, object> Payload { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the errors.
        /// </summary>
        public object Errors { get; set; }
    }
}
