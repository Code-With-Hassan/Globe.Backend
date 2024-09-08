using Globe.Core.Entities.Base;

namespace Globe.Shared.Entities
{
    public class RoleOrganizationsEntity : BaseEntity
    {
        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        public long RoleId { get; set; }

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        public RoleEntity Role { get; set; }

        /// <summary>
        /// Gets or sets the role id.
        /// </summary>
        public long OrganizationId { get; set; }

        /// <summary>
        /// Gets or sets the role.
        /// </summary>
        public OrganizationEntity Organization { get; set; }
    }
}
