using Globe.Shared.Models.Privileges;

namespace Globe.Account.Service.Services.PrivilegesService
{
    /// <summary>
    /// The privileges service.
    /// </summary>
    public interface IPrivilegesService
    {
        /// <summary>
        /// Gets the user privileges Model async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>An UserReadPrivilegesModel object.</returns>
        Task<UserReadPrivilegesModel> GetUserPrivilegesAsync(long userId);
    }
}