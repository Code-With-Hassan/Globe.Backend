using Globe.Core.Entities.Base;
using Microsoft.EntityFrameworkCore;

namespace Globe.Shared.Entities
{
    /// <summary>
    /// The role screen entity.
    /// </summary>
    public class RoleScreenEntity : BaseEntity
    {
        /// <summary>
        /// No privilege.
        /// </summary>
        public const int NO_PRIVILEGE = 0;

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
        /// Gets or sets the role id.
        /// </summary>
        public long RoleId { get; set; }

        /// <summary>
        /// Gets or sets the role.
        /// </summary>
        public RoleEntity Role { get; set; }

        /// <summary>
        /// Gets or sets the screen id.
        /// </summary>
        public long ScreenId { get; set; }

        /// <summary>
        /// Gets or sets the screen.
        /// </summary>
        [DeleteBehavior(DeleteBehavior.NoAction)]
        public ScreenEntity Screen { get; set; }

        /// <summary>
        /// 0 = Not Shown, 1 = admin, 2 = custom
        /// </summary>
        public int Privilege { get; set; }

        /// <summary>
        /// Have no privilege.
        /// </summary>
        /// <returns>A bool.</returns>
        public bool HasNoPrivilege()
        {
            return Privilege == NO_PRIVILEGE;
        }

        /// <summary>
        /// Have admin privilege.
        /// </summary>
        /// <returns>A bool.</returns>
        public bool HasAdminPrivilege()
        {
            return Privilege == ADMIN_PRIVILEGE;
        }

        /// <summary>
        /// Have custom privilege.
        /// </summary>
        /// <returns>A bool.</returns>
        public bool HasCustomPrivilege()
        {
            return Privilege == CUSTOM_PRIVILEGE;
        }

        /// <summary>
        /// Check if assigned privilege of role screen is READONLY_PRIVILEGE.
        /// </summary>
        /// <returns>A bool.</returns>
        public bool HasReadOnlyPrivilege()
        {
            return Privilege == READONLY_PRIVILEGE;
        }

        /// <summary>
        /// Have admin or custom privilege.
        /// </summary>
        /// <returns>A bool.</returns>
        public bool HasAdminCustomOrReadOnlyPrivilege()
        {
            return Privilege == ADMIN_PRIVILEGE || Privilege == CUSTOM_PRIVILEGE || Privilege == READONLY_PRIVILEGE;
        }
    }
}
