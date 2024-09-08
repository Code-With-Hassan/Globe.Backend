using Globe.Core.Entities.Base;

namespace Globe.Shared.Entities
{
    /// <summary>
    /// The user role mapping entity to resolve n-n relation between Users and Roles Entity.
    /// Roles are assigned to users.
    /// </summary>
    public class UserRoleEntity : BaseEntity
    {
        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        public UserEntity User { get; set; }

        /// <summary>
        /// Gets or sets the role id.
        /// </summary>
        public long RoleId { get; set; }

        /// <summary>
        /// Gets or sets the role.
        /// </summary>
        public RoleEntity Role { get; set; }
    }
}
