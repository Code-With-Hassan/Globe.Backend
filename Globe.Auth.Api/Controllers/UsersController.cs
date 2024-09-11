using Microsoft.AspNetCore.Mvc;
using Globe.Account.Service.Services.UserService;
using Globe.Account.Api.Extensions;

namespace Globe.Account.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UsersController : BaseController<UsersController>
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService, ILogger<UsersController> logger) : base(logger)
        {
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
