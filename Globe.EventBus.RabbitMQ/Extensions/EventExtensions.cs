using Globe.Core.AuditHelpers;
using Globe.Core.Entities.Base;
using Globe.Core.Repository;
using Globe.Core.Repository.impl;
using Globe.EventBus.RabbitMQ.Event;
using Globe.EventBus.RabbitMQ.Sender;
using Globe.Shared.Constants;

namespace Globe.EventBus.RabbitMQ.Extensions
{
    public static class EventExtensions
    {
        public static void SetAfterSaveEvent<T>(this IEventSender eventSender, IRepository<T> repository) where T : BaseEntity 
        {
            ((GenericRepository<T>)repository).AfterSave = (logs) =>
            {
                eventSender.SendEvent(new MQEvent<List<AuditEntry>>(RabbitMqQueuesConstants.AuditQueueName, (List<AuditEntry>)logs));
            };

        }
    }
}
