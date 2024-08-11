
using Globe.Audit.Api.Models;
using Globe.Core.Entities;

namespace Globe.Audit.Api.Helpers
{
    /// <summary>
    /// The audit entry converter.
    /// Converts business audits to audit entity.
    /// </summary>
    public static class AuditEntryConverter
    {
        /// <summary>
        /// Converts export log model to audit entity.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <param name="model">The export log model.</param>
        /// <returns>A Domain.Core.Entities.Audit object.</returns>
        public static AuditEntity ToAudit(string userName, ExportLogModel model)
        {
            var audit = new AuditEntity
            {
                AuditDateTimeUtc = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                AuditType = model.AuditType,
                AuditUser = userName,
                TableName = model.ScreenName,
                KeyValues = string.Empty,
                OldValues = string.Empty,
                NewValues = model.FilterExpression,
                ChangedColumns = string.Empty
            };
            return audit;
        }

        /// <summary>
        /// Associate OrganizationEntity with audits
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="auditId"></param>
        /// <returns></returns>
        public static AuditOrganizationEntity AssociateOrgToAudits(long orgId, long auditId)
        {
            return new AuditOrganizationEntity
            {
                OrganizationId = orgId,
                AuditId = auditId
            };
        }
    }
}
