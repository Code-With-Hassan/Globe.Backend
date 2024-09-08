using System.ComponentModel.DataAnnotations;

namespace Globe.Shared.Models.Privileges
{
    /// <summary>
    /// Base role Model.
    /// </summary> 
    public class RoleBaseModel
    {
        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the RoleDescription.
        /// </summary>
        [Required]
        public string RoleDescription { get; set; }

        /// <summary>
        /// Gets or sets DefaultApplication Id
        /// </summary>
        [Required]
        public long DefaultApplicationId { get; set; }

        /// <summary>
        /// Gets or sets AllowedApplication Ids.
        /// </summary>
        public List<long> AllowedApplicationIds { get; set; }

        /// <summary>
        /// Gets or sets RoleOrganization Ids.
        /// </summary>
        public long[] OrganizationIds { get; set; }

        /// <summary>
        /// Gets or sets the privileges.
        /// </summary>
        public List<PrivilegesModel> Privileges { get; set; } = new();

        /// <summary>
        /// Gets or sets the column privileges.
        /// </summary> 
        public List<RoleOrganizationsModel> RoleOrganizations { get; set; } = new();
    }
}
