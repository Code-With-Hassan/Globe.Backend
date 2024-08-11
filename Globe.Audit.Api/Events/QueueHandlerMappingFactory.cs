using Globe.Audit.Api.Database;
using Globe.Audit.Api.Events.Handlers;
using Globe.Audit.Api.Services;
using Globe.Core.Entities;
using Globe.Core.Repository;
using Globe.Core.Repository.impl;
using Globe.EventBus.RabbitMQ.Receiver;
using Globe.Shared.Constants;

namespace Globe.Audit.Api.Events
{
    /// <summary>
    /// The queue handler mapping factory.
    /// It is used to map each message que with specific event handler to take care of.
    /// </summary>
    public class QueueHandlerMappingFactory : IQueueHandlerMappingFactory
    {
        private readonly IRepository<AuditEntity> _auditRepository;
        private readonly IAuditService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueHandlerMappingFactory"/> class.
        /// </summary>
        /// <param name="provider">The service provider.</param>
        public QueueHandlerMappingFactory(IServiceProvider provider)
        {
            var scope = provider.CreateScope();
            var context = scope.ServiceProvider.GetService<AuditDbContext>();
            _auditRepository = new GenericRepository<AuditEntity>(context);
            _service = scope.ServiceProvider.GetService<IAuditService>();

        }

        /// <summary>
        /// Gets the queue to handler mapping.
        /// If a message que is required to be added in event bus, its event handler needs to be implemented and then,
        /// mapped in following mapping function.
        /// </summary>
        /// <returns>A Dictionary of event handlers.</returns>
        public Dictionary<string, IEventHandler> GetQueueToHandlerMapping()
        {
            var result = new Dictionary<string, IEventHandler>
            {
                [RabbitMqQueuesConstants.AuditQueueName] = new AuditEventHandler(_auditRepository, _service)
            };
            return result;
        }
    }
}
