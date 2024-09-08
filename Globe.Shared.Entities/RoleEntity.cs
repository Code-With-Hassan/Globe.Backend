using Globe.Core.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace Globe.Shared.Entities
{
    /// <summary>
    /// The role of a user. A user can have n-n relation with a role
    /// </summary>
    public class RoleEntity : BaseEntity
    {
        /// <summary>
        /// Gets or sets the role description.
        /// </summary>
        [Required]
        public string RoleDescription { get; set; }

        /// <summary>
        /// Gets or sets DefaultApplication Id
        /// </summary>
        [Required]
        public long DefaultApplicationId { get; set; }

        public ApplicationEntity DefaultApplication { get; set; }


        /// <summary>
        /// Gets or sets the user roles.
        /// </summary>
        public ICollection<UserRoleEntity> UserRoles { get; set; }


        /// <summary>
        /// Gets or sets the screens that are associated with a role.
        /// Along with their privileges.
        /// </summary>
        public ICollection<RoleScreenEntity> RoleScreens { get; set; }

        /// <summary>
        /// Gets or sets role applications
        /// </summary>
        public ICollection<RoleApplicationEntity> RoleApplications { get; set; }

        /// <summary>
        /// Gets or sets the roles organization
        /// </summary>
        public ICollection<RoleOrganizationsEntity> RoleOrganizations { get; set; }
    }
}
