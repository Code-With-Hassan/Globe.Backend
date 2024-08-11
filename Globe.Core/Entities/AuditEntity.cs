using Globe.Core.Entities.Base;

namespace Globe.Core.Entities
{
    /// <summary>
    /// The audit entity.
    /// </summary>
    public class AuditEntity : BaseEntity
    {
        /// <summary>
        /// Gets or sets the audit date time utc.
        /// </summary>
        public long AuditDateTimeUtc { get; set; }     /*Log time*/

        /// <summary>
        /// Gets or sets the audit type.
        /// </summary>
        public string AuditType { get; set; }           /*Create, Update, Delete or Export*/

        /// <summary>
        /// Gets or sets the audit user.
        /// </summary>
        public string AuditUser { get; set; }           /*Log User*/

        /// <summary>
        /// Gets or sets the audit user.
        /// </summary>
        //public long OganizationId { get; set; }

        /// <summary>
        /// Gets or sets the table name.
        /// </summary>
        public string TableName { get; set; }           /*Table where rows been 
                                                          created/updated/deleted*/
        /// <summary>
        /// Gets or sets the key values.
        /// </summary>
        public string KeyValues { get; set; }           /*Table Pk and it's values*/

        /// <summary>
        /// Gets or sets the old values.
        /// </summary>
        public string OldValues { get; set; }           /*Changed column name and old value*/

        /// <summary>
        /// Gets or sets the new values.
        /// </summary>
        public string NewValues { get; set; }           /*Changed column name 
                                                          and current value*/
        /// <summary>
        /// Gets or sets the changed columns.
        /// </summary>
        public string ChangedColumns { get; set; }      /*Changed column names*/
    }
}
