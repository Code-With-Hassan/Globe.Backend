using Globe.Api.Gateway.Services.Routes;
using Microsoft.AspNetCore.Mvc;

namespace Globe.Api.Gateway.Controllers.v1
{
    /// <summary>
    /// The services controller.
    /// </summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ServicesController : ControllerBase
    {
        private readonly IRoutesService _routesService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServicesController"/> class.
        /// </summary>
        public ServicesController(IRoutesService routesService)
        {
            _routesService = routesService;
        }

        /// <summary>
        /// Get all services base urls.
        /// </summary>
        /// <returns>An IActionResult.</returns>
        public IActionResult GetAll()
        {
            return Ok(_routesService.GetAll());
        }
    }
}
