namespace Globe.EventBus.RabbitMQ.Event
{
    /// <summary>
    /// The Message Queue event generic class.
    /// Encapsulates Queue name and models.
    /// Used by controllers to send events.
    /// </summary>
    public class MQEvent<TModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MQEvent{TModel}"/> class.
        /// </summary>
        /// <param name="queueName">The queue name.</param>
        /// <param name="model">The model.</param>
        public MQEvent(string queueName, TModel model)
        {
            QueueName = queueName;
            Model = model;
        }

        /// <summary>
        /// Gets or sets the queue name.
        /// </summary>
        public string QueueName { get; set; }

        /// <summary>
        /// Gets or sets the generic model property.
        /// </summary>
        public TModel Model { get; set; }
    }
}
