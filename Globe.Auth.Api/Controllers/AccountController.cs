using Globe.Account.Api.Extensions;
using Globe.Account.Service.Services.UserRegistrationService;
using Globe.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace Globe.Account.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : BaseController
    {
        private readonly IUserRegistrationService _userRegistrationService;

        public AccountController(IUserRegistrationService userRegistrationService)
        {
            _userRegistrationService = userRegistrationService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var result = await _userRegistrationService.RegisterUserAsync(model.Username, model.Email, model.Password);

            if (result)
            {
                return Ok(true);
            }
            return BadRequest(false, "Registration failed");
        }
    }
}
