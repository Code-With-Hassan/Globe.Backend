
namespace Globe.Shared.Models.Privileges
{
    /// <summary>
    /// The user read privileges Model.
    /// </summary>
    public class UserReadPrivilegesModel : UserReadModel
    {
        /// <summary>
        /// Gets or sets the list of screen privileges.
        /// </summary>
        public List<RoleScreenModel> ScreenPrivileges { get; set; }

        /// <summary>
        /// Gets or sets the allowed applications.
        /// </summary>
        public List<string> AllowedApplications { get; set; }

        /// <summary>
        /// Gets the allowed screens list.
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllowedScreensList()
        {
            List<string> allowedScreenNames = new List<string>();

            if (ScreenPrivileges != null)
            {
                allowedScreenNames = ScreenPrivileges
                    .Where(x => x.HasAdminCustomOrReadOnlyPrivilege())
                    .Select(x => x.ScreenName)
                    .ToList();
            }

            return allowedScreenNames;
        }


    }
}
