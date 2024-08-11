namespace Globe.Core.Entities.Base
{
    /// <summary>
    /// The base entity fo all the entities defined in the system. 
    /// This will ensure that all the entities have an ID, status, CreatedAt & UpdatedAt properties
    /// </summary>
    public class BaseEntity
    {
        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is active.
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// DateTime at which the record was created
        /// </summary>
        public long CreatedAt { get; set; }

        /// <summary>
        /// DateTime at which the record was updated
        /// </summary>
        public long UpdatedAt { get; set; }
    }
}
