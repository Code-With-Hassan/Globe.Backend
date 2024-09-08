using Globe.Shared.Models.Privileges;

namespace Globe.Account.Service.Services.PrivilegesService
{
    /// <summary>
    /// The super user privileges service.
    /// </summary>
    public interface ISuperUserPrivilegesService
    {
        /// <summary>
        /// Gets the user privileges Model async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>An UserReadPrivilegesModel object.</returns>
        Task<UserReadPrivilegesModel> GetSuperUserPrivilegesAsync(long userId);
    }
}
