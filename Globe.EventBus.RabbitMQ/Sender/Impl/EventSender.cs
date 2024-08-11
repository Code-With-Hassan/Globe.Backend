using Globe.EventBus.RabbitMQ.Config;
using Globe.EventBus.RabbitMQ.Event;
using Globe.Shared.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using System.Text;

namespace Globe.EventBus.RabbitMQ.Sender.Impl
{
    /// <summary>
    /// The event sender implementation.
    /// </summary>
    public class EventSender : IEventSender
    {
        private readonly ILogger _logger;
        private readonly string _hostname;
        private readonly string _username;
        private readonly string _password;
        private readonly int _retryDuration;
        private readonly int _retryCount;
        IConnection _connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventSender"/> class.
        /// </summary>
        /// <param name="rabbitMqOptions">The rabbit mq configuration options.</param>
        public EventSender(IOptions<RabbitMqConfiguration> rabbitMqOptions,
            ILogger<EventSender> logger)
        {
            _retryDuration = rabbitMqOptions.Value.RetryDuration;
            _retryCount = rabbitMqOptions.Value.RetryCount;
            _hostname = rabbitMqOptions.Value.Hostname;
            _username = rabbitMqOptions.Value.UserName;
            _password = EncryptionHelper.DecryptString(rabbitMqOptions.Value.Password);
            _logger = logger;
        }

        /// <summary>
        /// Sends the event.
        /// </summary>
        /// <param name="mqEvent">The mq event.</param>
        /// <returns>An EventSenderStatus enumeration value.</returns>
        public EventSenderStatus SendEvent<TModel>(MQEvent<TModel> mqEvent)
        {
            var factory = new ConnectionFactory()
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
                            nameof(EventSender),
                            ++retryCount);

                        return TimeSpan.FromSeconds(_retryDuration);
                    });

                policy.Execute(() =>
                {
                    _connection = factory.CreateConnection();
                });

                _logger.LogInformation(
                    "{Event}'s connection with eventbus established successfully after {retryCount} retrie(s)",
                    nameof(EventSender),
                    retryCount);

                using (var channel = _connection.CreateModel())
                {
                    channel.QueueDeclare(queue: mqEvent.QueueName,
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    var json = JsonConvert.SerializeObject(mqEvent);
                    var body = Encoding.UTF8.GetBytes(json);

                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    channel.BasicPublish(exchange: string.Empty,
                        routingKey: mqEvent.QueueName,
                        basicProperties: properties,
                        body: body);
                }
            }
            catch (Exception e)
            {
                // Log error message
                _logger.LogError(e, "{Message}: {Exception}", e.Message, e.ToString());
            }

            return EventSenderStatus.Success;
        }
    }
}
