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

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var token = await _authService.LoginAsync(model.Username, model.Password);
            if (token != null)
            {
                return Ok<string>(token);
            }
            return Unauthorized();
        }
    }
}
