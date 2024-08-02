using Globe.Auth.Api.Extensions;
using Globe.Auth.Service.Services.AuthService;
using Globe.Shared.Models;
using Globe.Shared.MVC.Resoures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Globe.Auth.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _logger = logger;
            _authService = authService;
        }

        /// <summary>
        /// Authenticate the user and generates JWT token.
        /// </summary>
        /// <returns>User login result.</returns>
        /// <response code="200">JWT token and user Object</response>
        /// <response code="404">Error in case the user was not found.</response>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            try
            {
                // Checking if the passed Model is valid
                if (!ModelState.IsValid || model == null)
                {
                    return BadRequest(MsgKeys.InvalidLoginCredentials);
                }

                var userResponse = await _authService.LoginAsync(model.Username, model.Password);
                
                if (userResponse != null)
                {
                    _logger.LogInformation("User logged in: {UserId} => {UserName}",
                                            userResponse.User.Id,
                                            userResponse.User.UserName);

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
