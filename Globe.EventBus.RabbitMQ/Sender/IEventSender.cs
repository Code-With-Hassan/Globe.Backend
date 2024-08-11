using Globe.EventBus.RabbitMQ.Event;

namespace Globe.EventBus.RabbitMQ.Sender
{
    /// <summary>
    /// The event sender status enumeration.
    /// </summary>
    public enum EventSenderStatus { Success, Error }
    /// <summary>
    /// The event sender interface.
    /// </summary>
    public interface IEventSender
    {
        /// <summary>
        /// Responsible for sending the event.
        /// </summary>
        /// <param name="msg">The message queue event.</param>
        /// <returns>An EventSenderStatus enumeration value.</returns>
        EventSenderStatus SendEvent<TModel>(MQEvent<TModel> msg);
    }
}
