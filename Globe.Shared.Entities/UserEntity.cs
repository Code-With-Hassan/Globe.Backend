namespace Globe.Shared.Entities
{
    using Globe.Core.Entities.Base;
    using System.ComponentModel.DataAnnotations;

    public class UserEntity : BaseEntity
    {
        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        [Required]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the user's email address.
        /// </summary>
        [Required]
        public string Email { get; set; }

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
        /// Gets or sets the PasswordResetTime.
        /// </summary>
        public long? PasswordResetTime { get; set; }

        /// <summary>
        /// Gets or sets the UserRoles.
        /// </summary>
        public ICollection<UserRoleEntity> UserRoles { get; set; }
    }
}
