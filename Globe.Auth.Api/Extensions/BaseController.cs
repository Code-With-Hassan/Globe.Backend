using Globe.Auth.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Globe.Account.Api.Extensions
{
    public abstract class BaseController : ControllerBase
    {
        /// <summary>
        /// Returns an Ok response with a standardized format.
        /// </summary>
        /// <typeparam name="T">Type of the response data.</typeparam>
        /// <param name="data">Response data.</param>
        /// <param name="message">Optional message.</param>
        /// <returns>Standardized Ok response.</returns>
        protected new IActionResult Ok<T>(T data, string message = "Request successful.")
        {
            var response = new ApiResponse<T>(true, data, message);
            return base.Ok(response);
        }

        /// <summary>
        /// Returns a BadRequest response with a standardized format.
        /// </summary>
        /// <typeparam name="T">Type of the response data.</typeparam>
        /// <param name="data">Response data.</param>
        /// <param name="message">Optional message.</param>
        /// <returns>Standardized BadRequest response.</returns>
        protected new IActionResult BadRequest<T>(T data, string message = "Request failed.")
        {
            var response = new ApiResponse<T>(false, data, message);
            return base.BadRequest(response);
        }

        /// <summary>
        /// Returns a BadRequest response with a standardized format for error messages.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Standardized BadRequest response.</returns>
        protected new IActionResult BadRequest(string message)
        {
            var response = new ApiResponse<string>(false, null, message);
            return base.BadRequest(response);
        }
    }
}
