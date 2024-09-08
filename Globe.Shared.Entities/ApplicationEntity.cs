using Globe.Core.Entities.Base;

namespace Globe.Shared.Entities
{
    /// <summary>
    /// The application entity.
    /// </summary>
    public class ApplicationEntity : BaseEntity
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Screens.
        /// </summary>
        public ICollection<ScreenEntity> Screens { get; set; }
    }
}
