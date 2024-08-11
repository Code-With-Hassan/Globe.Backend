using Globe.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Generic;

namespace Globe.Core.AuditHelpers
{
    /// <summary>
    /// The audit helper.
    /// </summary>
    public class AuditHelper
    {
        readonly DbContext Db;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditHelper"/> class.
        /// </summary>
        /// <param name="db">The auditable db context.</param>
        public AuditHelper(DbContext db)
        {
            Db = db;
        }

        /// <summary>
        /// Adds the audit logs.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <param name="organizationIds">The organization Ids.</param>
        public List<AuditEntry> CreateAuditLogs(string userName, List<long> organizationIds)
        {
            Db.ChangeTracker.DetectChanges();
            var auditEntries = new List<AuditEntry>();
            foreach (EntityEntry entry in Db.ChangeTracker.Entries())
            {
                if (entry.Entity is AuditEntity || entry.Entity is AuditOrganizationEntity ||
                    entry.State == EntityState.Detached ||
                    entry.State == EntityState.Unchanged)
                {
                    continue;
                }
                var auditEntry = new AuditEntry(entry, userName, organizationIds);
                auditEntries.Add(auditEntry);
            }

            return auditEntries;
        }

    }
}
