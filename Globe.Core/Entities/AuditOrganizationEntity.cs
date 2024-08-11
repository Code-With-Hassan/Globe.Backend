using Globe.Core.Entities.Base;

namespace Globe.Core.Entities
{
    public class AuditOrganizationEntity : BaseEntity
    {
        public long OrganizationId { get; set; }

        public long AuditId { get; set; }

        public AuditEntity Audit { get; set; }
    }
}
