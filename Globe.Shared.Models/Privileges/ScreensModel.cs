using System.ComponentModel.DataAnnotations;

namespace Globe.Shared.Models.Privileges
{
    /// <summary>
    /// The screens Model.
    /// </summary>
    public class ScreensModel
    {
        /// <summary>
        /// Gets or sets the screen Id.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the ScreenName.
        /// </summary>
        [MaxLength(50)]
        public string ScreenName { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        public int ScreenPrivilege { get; set; }

        /// <summary>
        /// Gets or sets the application.
        /// </summary>
        public ApplicationModel Application { get; set; }
    }
}
