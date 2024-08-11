using Globe.Audit.Api.Models;
using Globe.Audit.Api.Services;
using Globe.Core.Entities;
using Globe.Shared.Models;
using Globe.Shared.MVC;
using Globe.Shared.MVC.Resoures;
using Microsoft.AspNetCore.Mvc;

namespace Globe.Audit.Api.Controllers
{
    /// <summary>
    /// The Audit api controller.
    /// Following controller will be used to get and filter audit logs.
    /// When user performs exports on a particular screen a post call is submitted to log export operation.
    /// </summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuditController : PilotBaseController
    {
        private readonly IAuditService _service;
        private readonly IConfiguration _config;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="AuditController"/> class.
        /// </summary>
        public AuditController(IAuditService service, IConfiguration config)
        {
            _service = service;
            _config = config;
        }

        /// <summary>
        /// Gets the Audit logs.
        /// </summary>
        /// <param name="queryStringParams">The query string params.</param>
        /// <returns>Audit logs.</returns>
        [HttpGet]
        public ActionResult<Response> GetAudits([FromQuery] QueryStringParams? queryStringParams)
        {
            PagedResult<AuditEntity> result;
            try
            {
                result = _service.GetPagedResult(queryStringParams);
            }
            catch (ArgumentException e)
            {
                return BadRequest(new
                {
                    Errors = e,
                    e.Message
                });
            }
            return Ok(result);
        }

        /// <summary>
        /// Logs the export audit.
        /// </summary>
        /// <param name="model">The export log model containing export information.</param>
        /// <returns>Return ok response if logged successfully.</returns>
        [HttpPost("export")]
        public IActionResult PostAudit(ExportLogModel model)
        {
            // Checking if the passed Model is valid
            if (!ModelState.IsValid || model == null)
            {
                return BadRequest(MsgKeys.InvalidInputParameters);
            }

            try
            {
                model = _service.Create(model);
                return Ok(model);
            }
            catch (Exception ex)
            {
                // Show Error message
                return BadRequest(new
                {
                    Errors = ex,
                    ex.Message
                });
            }
        }

        /// <summary>
        /// Endpoint to get all active Users.
        /// </summary>
        /// <returns>A list of all active Global messages</returns>
        //[HttpGet("users")]
        //public IActionResult GetUsers()
        //{
        //    try
        //    {
        //        var result = _service.GetUsers();
        //        return Ok(new { Items = result });
        //    }
        //    catch (Exception e)
        //    {
        //        // Show Error message
        //        return BadRequest(new
        //        {
        //            Errors = e,
        //            e.Message
        //        });
        //    }
        //}

        /// <summary>
        /// Endpoint to get all active Users.
        /// </summary>
        /// <returns>A list of all active Global messages</returns>
        [HttpGet("users/tables")]
        public IActionResult GetTables()
        {
            try
            {
                var result = _service.GetTables();
                return Ok(new { Items = result });
            }
            catch (Exception e)
            {
                // Show Error message
                return BadRequest(new
                {
                    Errors = e,
                    e.Message
                });
            }
        }
    }
}
