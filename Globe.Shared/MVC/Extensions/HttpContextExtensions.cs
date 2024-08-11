using Globe.Shared.Constants;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace Globe.Shared.MVC.Extensions
{
    /// <summary>
    /// The http context extensions.
    /// </summary>
    public static class HttpContextExtensions
    {
        static readonly JwtSecurityTokenHandler _handler = new();

        /// <summary>
        /// Gets the user name.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>A string.</returns>
        public static string GetUserName(this HttpContext context)
        {
            var result = context?.Request.Headers[IAuthConstants.UserName].ToString();
            return string.IsNullOrWhiteSpace(result) ? "System" : result;
        }

        /// <summary>
        /// Gets the user Id.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static int GetUserId(this HttpContext context)
        {
            var result = context.Request.Headers[IAuthConstants.UserId].ToString();
            return string.IsNullOrWhiteSpace(result) ? 0 : Convert.ToInt32(result);
        }

        /// <summary>
        /// Gets the user Id.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static List<long> GetOrganizationIds(this HttpContext context)
        {
            // Null check is necessary for RabittMq calls.
            var result = context?.Request.Headers[IAuthConstants.OrganizationIds].ToString();
            return string.IsNullOrWhiteSpace(result) ? new List<long>() : JsonConvert.DeserializeObject<List<long>>(result);
        }

        /// <summary>
        /// Gets the corelation id.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>A string.</returns>
        public static string GetCorelationId(this HttpContext context)
        {
            var result = context.Request.Headers[IAuthConstants.CorelationId].ToString();
            return string.IsNullOrWhiteSpace(result) ? Guid.NewGuid().ToString() : result;
        }

        /// <summary>
        /// Gets the corelation id.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>A string.</returns>
        public static bool IsSuperUser(this HttpContext context)
        {
            var result = context.Request.Headers[IAuthConstants.IsSuperUser].ToString();
            return !string.IsNullOrWhiteSpace(result) && Convert.ToBoolean(result);
        }
    }
}
