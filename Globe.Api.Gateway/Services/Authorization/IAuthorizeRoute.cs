using System.Collections.Generic;
using System.Threading.Tasks;

namespace Globe.Api.Gateway.Services.Authorization
{
    /// <summary>
    /// The route authorization interface.
    /// </summary>
    public interface IAuthorizeRoute
    {
        /// <summary>
        /// Checks if request is allowed
        /// </summary>
        /// <param name="allowedApplicationsList"></param>
        /// <param name="allowedScreensList">The List of allowed screens.</param>
        /// <param name="path">The path.</param>
        /// <param name="method">The method.</param>
        /// <returns>A Task.</returns>
        Task<bool> IsAllowedAsync(List<string> allowedApplicationsList, List<string> allowedScreensList, string path, string method);
    }
}