namespace Globe.EventBus.RabbitMQ.Config
{
    /// <summary>
    /// The RabbitMQ configuration settings.
    /// These settings are required by <see cref="Sender.Impl.EventSender"/> and <see cref="Receiver.Impl.EventListener"/>,
    /// in order to connect to RabbitMQ event bus.
    /// (RabbitMQ event bus runs as background service)
    /// </summary>
    public class RabbitMqConfiguration
    {
        /// <summary>
        /// Gets or sets the hostname.
        /// This is the URL on which RabbitMQ event bus is listening.
        /// </summary>
        public string Hostname { get; set; }

        /// <summary>
        /// Gets or sets the user name.
        /// The username required to login RabbitMQ management console.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// The password required to login RabbitMQ management console.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the RetryCount.
        /// The policy that will wait and retry RetryCount times on rabitMq connection
        /// </summary>
        public int RetryCount { get; set; }

        /// <summary>
        /// Gets or sets the RetryDuration in seconds.
        /// The duration to wait on each retry
        /// </summary>
        public int RetryDuration { get; set; }
    }
}
