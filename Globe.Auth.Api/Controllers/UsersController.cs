using Microsoft.AspNetCore.Mvc;
using Globe.Auth.Api.Extensions;
using Globe.Auth.Service.Services.UserService;

namespace Globe.Auth.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : BaseController
    {
        private readonly IUserService _userService;
        private readonly ILogger<AuthController> _logger;

        public UsersController(IUserService userService, ILogger<AuthController> logger)
        {
            _logger = logger;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                return Ok(await _userService.GetAllUsers());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest("Something went wrong");
            }
        }
    }
}
