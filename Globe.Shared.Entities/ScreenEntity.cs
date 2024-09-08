using Globe.Core.Entities.Base;

namespace Globe.Shared.Entities
{
    /// <summary>
    /// The screen entity.
    /// </summary>
    public class ScreenEntity : BaseEntity
    {
        /// <summary>
        /// Gets or sets the screen name.
        /// </summary>
        public string ScreenName { get; set; }

        /// <summary>
        /// Gets or sets the application id.
        /// </summary>
        public long ApplicationId { get; set; }

        /// <summary>
        /// Get or Set Screen Privilege
        /// </summary>
        public bool ScreenPrivilege { get; set; } = true;

        /// <summary>
        /// Gets or sets the application.
        /// </summary>
        public ApplicationEntity Application { get; set; }

        /// <summary>
        /// Gets or sets the role screens.
        /// </summary>
        public ICollection<RoleScreenEntity> RoleScreens { get; set; }
    }
}
