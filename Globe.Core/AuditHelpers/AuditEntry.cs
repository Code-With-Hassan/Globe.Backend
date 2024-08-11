using Globe.Core.Constants;
using Globe.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json;

namespace Globe.Core.AuditHelpers
{
    /// <summary>
    /// The audit entry.
    /// </summary>
    public class AuditEntry
    {
        /// <summary>
        /// Gets or sets the audit type.
        /// </summary>
        public string AuditType { get; set; }

        /// <summary>
        /// Gets or sets the audit user.
        /// </summary>
        public string AuditUser { get; set; }

        /// <summary>
        /// Gets or sets the audit user.
        /// </summary>
        //public long OrganizationId { get; set; } = 0;

        /// <summary>
        /// Gets or sets the table name.
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Gets the key values.
        /// </summary>
        public Dictionary<string, object> KeyValues { get; } = new Dictionary<string, object>();

        /// <summary>
        /// Gets the old values.
        /// </summary>
        public Dictionary<string, object> OldValues { get; } = new Dictionary<string, object>();

        /// <summary>
        /// Gets the new values.
        /// </summary>
        public Dictionary<string, object> NewValues { get; } = new Dictionary<string, object>();

        /// <summary>
        /// Gets the changed columns.
        /// </summary>
        public List<string> ChangedColumns { get; } = new List<string>();

        /// <summary>
        /// Get or Set OrganizationIds
        /// </summary>
        public List<long> OrganizationIds { get; set; } = new List<long>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditEntry"/> class.
        /// </summary>
        /// <param name="entry">The entity entry.</param>
        /// <param name="auditUser">The audit user.</param>
        public AuditEntry(EntityEntry entry, string auditUser, List<long> organizationId)
        {
            OrganizationIds = organizationId;
            AuditUser = auditUser;
            SetChanges(entry);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditEntry"/> class.
        /// </summary>
        public AuditEntry() { }

        /// <summary>
        /// Sets the changes.
        /// </summary>
        private void SetChanges(EntityEntry entry)
        {
            TableName = entry.Metadata.GetTableName();
            var schema = entry.Metadata.GetSchema();

            var dbValues = entry.State == EntityState.Modified ? entry.GetDatabaseValues() : null;

            var storeObjectIdentifier = StoreObjectIdentifier.Table(TableName, schema);

            foreach (PropertyEntry property in entry.Properties)
            {
                string propertyName = property.Metadata.Name;
                string dbColumnName = property.Metadata.GetColumnName(storeObjectIdentifier);

                if (property.Metadata.IsPrimaryKey())
                {
                    KeyValues[propertyName] = property.CurrentValue;
                    continue;
                }

                switch (entry.State)
                {
                    case EntityState.Added:
                        NewValues[propertyName] = property.CurrentValue;
                        AuditType = AuditTypes.Create;
                        break;

                    case EntityState.Deleted:
                        OldValues[propertyName] = property.OriginalValue;
                        AuditType = AuditTypes.Delete;
                        break;

                    case EntityState.Modified:
                        if (property.IsModified)
                        {
                            var oldValue = dbValues != null ? dbValues[propertyName] : property.OriginalValue;
                            var newValue = property.CurrentValue;

                            if (oldValue != null && !oldValue.Equals(newValue)) ChangedColumns.Add(dbColumnName);

                            OldValues[propertyName] = oldValue;
                            NewValues[propertyName] = newValue;

                            AuditType = AuditTypes.Update;
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Creates an Audit object.
        /// </summary>
        /// <returns>An Audit object.</returns>
        public AuditEntity ToAudit()
        {
            var audit = new AuditEntity
            {
                AuditDateTimeUtc = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                AuditType = AuditType,
                AuditUser = AuditUser,
                //  OganizationId = OrganizationId,
                TableName = TableName,
                KeyValues = JsonConvert.SerializeObject(KeyValues),
                OldValues = OldValues.Count == 0 ?
                              string.Empty : JsonConvert.SerializeObject(OldValues),
                NewValues = NewValues.Count == 0 ?
                              string.Empty : JsonConvert.SerializeObject(NewValues),
                ChangedColumns = ChangedColumns.Count == 0 ?
                                   string.Empty : JsonConvert.SerializeObject(ChangedColumns)
            };

            return audit;
        }
    }
}
