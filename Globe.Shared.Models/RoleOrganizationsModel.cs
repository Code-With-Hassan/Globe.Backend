namespace Globe.Shared.Models
{
    public class RoleOrganizationsModel
    {
        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        public long RoleId { get; set; }

        /// <summary>
        /// Gets or sets the role id.
        /// </summary>
        public long OrganizationId { get; set; }

        /// <summary>
        /// Gets or sets the role.
        /// </summary>
        public OrganizationModel Organization { get; set; }
    }
}
