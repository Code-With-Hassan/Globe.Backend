using Globe.Account.Api.Extensions;
using Globe.Account.Service.Services.UserRegistrationService;
using Globe.Shared.Models;
using Globe.Shared.MVC.Resoures;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Globe.Account.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AccountController : BaseController<AccountController>
    {
        private readonly IUserRegistrationService _userRegistrationService;

        public AccountController(ILogger<AccountController> logger,
                                    IUserRegistrationService userRegistrationService) : base(logger)
        {
            _userRegistrationService = userRegistrationService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            try
            {
                // Checking if the passed Model is valid
                if (!ModelState.IsValid || model == null)
                    return BadRequest(new { Message = MsgKeys.InvalidInputParameters });

                var result = await _userRegistrationService.RegisterUserAsync(model.Username, model.Email, model.Password);

                if (result.Error != null)
                    return BadRequest(new { Message = MsgKeys.UserRegistrationFailed, Errors = _userRegistrationService.GetErrorMessageDictionary(result.Error.Message) });

                // If registration failed
                if (!result.IdentityResult.Succeeded)
                {
                    var dictionary = new Dictionary<string, string>();

                    // Collect all errors in a dictionary
                    foreach (IdentityError error in result.IdentityResult.Errors)
                    {
                        dictionary[error.Code] = error.Description;
                    }

                    // Passing the errors dictionary to the json response
                    return BadRequest(new { Message = MsgKeys.UserRegistrationFailed, Errors = dictionary });
                }

                return Ok(result.User);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(new { Message = MsgKeys.SomeThingWentWrong });
            }
        }
    }
}
