using Globe.Core.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace Globe.Shared.Entities
{
    public class OrganizationEntity : BaseEntity
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the phone.
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        [Required]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        [Required]
        public AddressEntity Address { get; set; }

        /// <summary>
        /// Gets or sets the Organizations of role.
        /// </summary>
        public ICollection<RoleOrganizationsEntity> RoleOrganizations { get; set; }

    }
}
