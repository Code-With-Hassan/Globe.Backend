using Globe.Auth.Api.Extensions;
using Globe.Auth.Service.Services.AuthService;
using Globe.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace Globe.Auth.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            try
            {
                var userResponse = await _authService.LoginAsync(model.Username, model.Password);
                if (userResponse != null)
                {
                    return Ok(userResponse);
                }
                return Unauthorized();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}
