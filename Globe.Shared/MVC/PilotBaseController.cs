using Globe.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace Globe.Shared.MVC
{
    /// <summary>
    /// The pilot base controller.
    /// Base Controller for all the micro services.
    /// </summary>
    public class PilotBaseController : ControllerBase
    {
        /// <summary>
        /// Overrides Ok Response.
        /// </summary>
        /// <param name="value">The value object.</param>
        /// <returns>An OkObjectResult.</returns>
        public override OkObjectResult Ok(object value)
        {
            return base.Ok(new Response()
            {
                Status = Models.Response.RequestStatus.Success,
                Payload = value.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .ToDictionary(prop => prop.Name, prop => prop.GetValue(value, null))
            });
        }

        /// <summary>
        /// Overrides Ok Response.
        /// </summary>
        /// <param name="message">The message to be returned.</param>
        /// <returns>An OkObjectResult.</returns>
        protected OkObjectResult Ok(string message)
        {
            return base.Ok(new Response()
            {
                Status = Models.Response.RequestStatus.Success,
                Message = message
            });
        }

        /// <summary>
        /// Overrides Ok Response.
        /// </summary>
        /// <param name="value">The value object.</param>
        /// <param name="message">The message to be returned.</param>
        /// <returns>An OkObjectResult.</returns>
        protected OkObjectResult Ok(object value, string message)
        {
            return base.Ok(new Response()
            {
                Status = Models.Response.RequestStatus.Success,
                Message = message,
                Payload = value.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .ToDictionary(prop => prop.Name, prop => prop.GetValue(value, null))
            });
        }

        /// <summary>
        /// Overrides Bad Request.
        /// </summary>
        /// <param name="value">The value object.</param>
        /// <returns>A BadRequestObjectResult.</returns>
        public override BadRequestObjectResult BadRequest(object value)
        {
            if (value.GetType() != typeof(string))
            {
                Dictionary<string, object> errorDictionary = value.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .ToDictionary(prop => prop.Name, prop => prop.GetValue(value, null));

                return base.BadRequest(new Response()
                {
                    Status = Models.Response.RequestStatus.Error,
                    Message = errorDictionary["Message"].ToString(),
                    Errors = errorDictionary["Errors"]
                });
            }
            return base.BadRequest(new Response()
            {
                Status = Models.Response.RequestStatus.Error,
                Message = value.ToString(),
                Errors = null
            });
        }
    }
}
