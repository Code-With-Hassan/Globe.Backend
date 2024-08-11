using System.Collections.Generic;

namespace Globe.EventBus.RabbitMQ.Receiver
{
    /// <summary>
    /// The queue handler mapping factory interface.
    /// It is used to map each message que with specific event handler to take care of.
    /// </summary>
    public interface IQueueHandlerMappingFactory
    {
        /// <summary>
        /// Gets the queue to handler mapping.
        /// </summary>
        /// <returns>A Dictionary of event handlers.</returns>
        Dictionary<string, IEventHandler> GetQueueToHandlerMapping();
    }
}
