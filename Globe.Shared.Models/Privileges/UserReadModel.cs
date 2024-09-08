using System.ComponentModel.DataAnnotations;

namespace Globe.Shared.Models.Privileges
{
    /// <summary>
    /// The user read Model.
    /// </summary>
    public class UserReadModel
    {
        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether user is active or not.
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// Gets or sets the Super User.
        /// </summary>
        public bool IsSuperUser { get; set; }

        /// <summary>
        /// Gets or sets the user's Locked.
        /// </summary>
        public bool IsLocked { get; set; }

        /// <summary>
        /// Gets or sets the last user login time.
        /// </summary>
        public long? LastLoggedIn { get; set; }

        /// <summary>
        /// Gets or sets the user's email.
        /// </summary>
        [Required]
        [MaxLength(256)]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the PasswordResetTime.
        /// </summary>
        public long? PasswordResetTime { get; set; }

        /// <summary>
        /// Gets or sets the user's role ids.
        /// </summary>
        public List<long> RoleIds { get; set; }

        /// <summary>
        /// Gets or sets the user roles.
        /// </summary>
        public IEnumerable<RoleBaseModel> Roles { get; set; }
    }
}
