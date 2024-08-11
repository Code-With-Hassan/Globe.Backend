namespace Globe.EventBus.RabbitMQ.Receiver
{
    /// <summary>
    /// The corresponding message que event handler interface. 
    /// </summary>
    public interface IEventHandler
    {
        /// <summary>
        /// Handlers the event.
        /// </summary>
        /// <param name="eventMsg">The event msg.</param>
        void HandlerEvent(string eventMsg);
    }
}
