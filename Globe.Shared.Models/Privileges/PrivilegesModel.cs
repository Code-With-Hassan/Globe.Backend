using System.ComponentModel.DataAnnotations;

namespace Globe.Shared.Models.Privileges
{
    public class PrivilegesModel
    {
        /// <summary>
        /// Gets or sets the Screen Id.
        /// </summary>
        [Required]
        public long ScreenId { get; set; }

        /// <summary>
        /// Gets or sets the Screen Privileges.
        /// 0 = None, 1 = Admin, 2 = Custom, 3=ReadOnly
        /// </summary>
        [Required]
        public int ScreenPrivileges { get; set; }
    }
}
