
namespace Globe.Shared.Entities
{
    public class UserOrganizationsEntity
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
        public long OrganizationId { get; set; }

        /// <summary>
        /// Gets or sets the role.
        /// </summary>
        public OrganizationEntity Organization { get; set; }
    }
}
