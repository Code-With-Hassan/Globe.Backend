using Globe.Account.Service.Data;
using Globe.Account.Service.Services.AuthService;
using Globe.Shared.Models.ResponseDTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Globe.Account.Service.Services.AuthService.Impl
{
    public class AuthService : IAuthService
    {
        private readonly ILogger<AuthService> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        public AuthService(ILogger<AuthService> logger,
                            UserManager<IdentityUser> userManager,
                            IConfiguration configuration,
                            ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<LoginDTO> LoginAsync(string username, string password)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(username);

                if (user is null)
                    throw new Exception("Invalid username");

                if (user != null && await _userManager.CheckPasswordAsync(user, password))
                {
                    await _dbContext.ApplicationUser.ExecuteUpdateAsync(usr => usr.SetProperty(p => p.lastLoggedIn, DateTime.UtcNow));
                    // Retrieve the roles for the user
                    var userRoles = await _userManager.GetRolesAsync(user);

                    LoginDTO response = new LoginDTO
                    {
                        Token = GenerateJwtToken(user, userRoles),
                        User = user,
                        UserOrganizations = new()
                    };
                    return response;
                }

                throw new Exception("Invalid username or password");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw ex;
            }
        }

        private string GenerateJwtToken(IdentityUser user, IList<string> userRoles)
        {
            IConfigurationSection jwtSettings = _configuration.GetSection("Jwt");
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings["Key"]));
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            List<Claim> claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            claims.AddRange(userRoles.Select(x => new Claim(ClaimTypes.Role, x)));

            JwtSecurityToken token = new JwtSecurityToken(
                notBefore: DateTime.UtcNow,
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["ExpireMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}