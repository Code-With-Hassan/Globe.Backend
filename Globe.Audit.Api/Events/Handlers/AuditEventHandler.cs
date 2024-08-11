using Globe.Audit.Api.Services;
using Globe.Core.AuditHelpers;
using Globe.Core.Entities;
using Globe.Core.Repository;
using Globe.EventBus.RabbitMQ.Event;
using Globe.EventBus.RabbitMQ.Receiver;
using Newtonsoft.Json;

namespace Globe.Audit.Api.Events.Handlers
{
    /// <summary>
    /// The audit event handler.
    /// </summary>
    public class AuditEventHandler : IEventHandler
    {
        private readonly IRepository<AuditEntity> _auditRepository;
        private readonly IAuditService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditEventHandler"/> class.
        /// </summary>
        /// <param name="auditRepository">The Audit user repository.</param>
        /// <param name="service"></param>
        public AuditEventHandler(IRepository<AuditEntity> auditRepository, IAuditService service)
        {
            _auditRepository = auditRepository;
            _service = service;
        }

        /// <summary>
        /// Handles the audit event.
        /// It converts each model (Created, Updated or Deleted) to audit model and save "audit log" inside a database,
        /// using provided audit repository.
        /// </summary>
        /// <param name="eventMsg">The event msg.</param>
        public void HandlerEvent(string eventMsg)
        {
            var eventObj = JsonConvert.DeserializeObject<MQEvent<List<AuditEntry>>>(eventMsg);

            var auditEntries = eventObj.Model;

            //if auditList is greater then 0 then get first OrganizationId
            //else set organizationId=0
            // organizationId refence in audit entity table
            //svar organizationId = auditEntries.Count > 0 ? auditEntries.FirstOrDefault().OrganizationId : 0;

            foreach (var auditEntry in auditEntries)
            {
                var entity = auditEntry.ToAudit();
                _auditRepository.Insert(entity);
                _service.AddAuditTable(entity);
                _auditRepository.SaveChanges(nameof(AuditEventHandler), new());
                _service.AssociateOrganization(entity.Id, auditEntry.OrganizationIds);
            }
        }
    }
}
