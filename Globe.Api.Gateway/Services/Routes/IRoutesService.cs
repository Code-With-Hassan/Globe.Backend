using System.Collections.Generic;

namespace Globe.Api.Gateway.Services.Routes
{
    /// <summary>
    /// The routes service interface.
    /// </summary>
    public interface IRoutesService
    {
        /// <summary>
        /// Gets all the routes.
        /// </summary>
        /// <returns>A list of string.</returns>
        List<string> GetAll();
    }
}