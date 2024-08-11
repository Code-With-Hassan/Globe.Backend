using Globe.EventBus.RabbitMQ.Config;
using Globe.Shared.Helpers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;

namespace Globe.EventBus.RabbitMQ.Receiver.Impl
{
    /// <summary>
    /// The event listener that runs as background service.
    /// </summary>
    public class EventListener : BackgroundService
    {
        private readonly ILogger _logger;
        private IModel _channel;
        private IConnection _connection;

        private Dictionary<string, EventConsumer> _consumers;
        private readonly string _hostname;
        private readonly string _username;
        private readonly string _password;
        private readonly int _retryDuration;
        private readonly int _retryCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventListener"/> class.
        /// </summary>
        /// <param name="rabbitMqOptions">The rabbit mq options.</param>
        /// <param name="eventHandlerFactory">The event handler factory.</param>
        public EventListener(IOptions<RabbitMqConfiguration> rabbitMqOptions,
            IQueueHandlerMappingFactory eventHandlerFactory,
               ILogger<EventListener> logger)
        {
            _retryDuration = rabbitMqOptions.Value.RetryDuration;
            _retryCount = rabbitMqOptions.Value.RetryCount;
            _hostname = rabbitMqOptions.Value.Hostname;
            _username = rabbitMqOptions.Value.UserName;
            var decryptedPassword = EncryptionHelper.DecryptString(rabbitMqOptions.Value.Password);
            _password = decryptedPassword;
            _consumers = new Dictionary<string, EventConsumer>();
            _logger = logger;

            InitializeRabbitMqListener(eventHandlerFactory);
        }

        /// <summary>
        /// Executes the async.
        /// </summary>
        /// <param name="stoppingToken">The stopping token.</param>
        /// <returns>A Task.</returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000 * 5, stoppingToken);
            }
        }

        /// <summary>
        /// Initializes the rabbit mq listener.
        /// </summary>
        /// <param name="eventHandlerFactory">The event handler factory.</param>
        private void InitializeRabbitMqListener(IQueueHandlerMappingFactory eventHandlerFactory)
        {
            var factory = new ConnectionFactory
            {
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(_retryDuration),
                HostName = _hostname,
                UserName = _username,
                Password = _password
            };
            try
            {
                int retryCount = 0;

                //retry rabitMq connection on failure exception : BrokerUnreachableException,SocketException,ConnectFailureException etc
                var policy = RetryPolicy.Handle<Exception>()
                    .WaitAndRetry(_retryCount, retryAttempt =>
                    {
                        _logger.LogInformation(
                            "{Event} is attempting to connect to...! Retry Count is: {retryCount}",
                            nameof(EventListener),
                            ++retryCount);

                        return TimeSpan.FromSeconds(_retryDuration);
                    });

                policy.Execute(() =>
                {
                    _connection = factory.CreateConnection();
                });

                _logger.LogInformation(
                    "{Event}'s connection with eventbus established successfully after {retryCount} retrie(s)",
                    nameof(EventListener),
                    retryCount);

                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
                _channel = _connection.CreateModel();

                Dictionary<string, IEventHandler> handlers = eventHandlerFactory.GetQueueToHandlerMapping();
                foreach (var kvp in handlers)
                {
                    _channel.QueueDeclare(queue: kvp.Key,
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    _consumers[kvp.Key] = new EventConsumer(kvp.Key, _channel, kvp.Value, _logger);
                }
            }
            catch (Exception e)
            {
                // Log error message
                _logger.LogError(e, "{Message}: {Exception}", e.Message, e.ToString());
            }
        }

        /// <summary>
        /// RabbitMQ connection shutdown.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The shutdown event arguments.</param>
        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e) { }

        /// <summary>
        /// Disposes the RabbitMQ after closing channel and connection.
        /// </summary>
        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}
