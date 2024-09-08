using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.ComponentModel.DataAnnotations;

namespace Globe.Shared.Models.Privileges
{
    /// <summary>
    /// The role screen Model.
    /// </summary>
    public class RoleScreenModel
    {
        /// <summary>
        /// Admin privilege.
        /// </summary>
        public const int ADMIN_PRIVILEGE = 1;

        /// <summary>
        /// Custom privilege.
        /// </summary>
        public const int CUSTOM_PRIVILEGE = 2;

        /// <summary> 
        /// ReadOnly privilege.
        /// </summary>
        public const int READONLY_PRIVILEGE = 3;

        /// <summary>
        /// No privilege.
        /// </summary>
        public const int NO_PRIVILEGE = 0;

        /// <summary>
        /// Gets or sets the role screen Id.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the Screen Name.
        /// </summary>
        public string ScreenName { get; set; }

        /// <summary>
        /// Gets or sets the Screen Privileges.
        /// 0 = None, 1 = Admin, 2 = Custom, 3 = ReadOnly
        /// </summary>
        [Required]
        public int ScreenPrivileges { get; set; }

        /// <summary>
        /// Gets or sets the Role Id.
        /// </summary>
        [Required]
        public long RoleId { get; set; }

        /// <summary>
        /// Gets or sets the Screen Id.
        /// </summary>
        [Required]
        public long ScreenId { get; set; }

        /// <summary>
        /// Gets or sets the application.
        /// </summary>
        public ApplicationModel Application { get; set; }

        // public List<AdminReportsModel> Reports { get; set; }
        /// <summary>
        /// Check if assigned privilege of role screen is NO_PRIVILEGE.
        /// </summary>
        /// <returns>A bool.</returns>
        public bool HasNoPrivilege()
        {
            return ScreenPrivileges == NO_PRIVILEGE;
        }

        /// <summary>
        /// Check if assigned privilege of role screen is ADMIN_PRIVILEGE.
        /// </summary>
        /// <returns>A bool.</returns>
        public bool HasAdminPrivilege()
        {
            return ScreenPrivileges == ADMIN_PRIVILEGE;
        }

        /// <summary>
        /// Check if assigned privilege of role screen is CUSTOM_PRIVILEGE.
        /// </summary>
        /// <returns>A bool.</returns>
        public bool HasCustomPrivilege()
        {
            return ScreenPrivileges == CUSTOM_PRIVILEGE;
        }

        /// <summary>
        /// Check if assigned privilege of role screen is READONLY_PRIVILEGE.
        /// </summary>
        /// <returns>A bool.</returns>
        public bool HasReadOnlyPrivilege()
        {
            return ScreenPrivileges == READONLY_PRIVILEGE;
        }

        /// <summary>
        /// Check if assigned privilege of role screen is ADMIN_PRIVILEGE or CUSTOM_PRIVILEGE.
        /// </summary>
        /// <returns>A bool.</returns>
        public bool HasAdminCustomOrReadOnlyPrivilege()
        {
            return ScreenPrivileges == ADMIN_PRIVILEGE || ScreenPrivileges == CUSTOM_PRIVILEGE || ScreenPrivileges == READONLY_PRIVILEGE;
        }
    }
}
