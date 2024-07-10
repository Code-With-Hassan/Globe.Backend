using Globe.Auth.Service.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Globe.Auth.Service.Services.AuthService.Impl
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<IdentityUser> userManager,
                            IConfiguration configuration,
                            ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<string> LoginAsync(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user != null && await _userManager.CheckPasswordAsync(user, password))
            {
                await _dbContext.ApplicationUser.ExecuteUpdateAsync(usr => usr.SetProperty(p => p.lastLoggedIn, DateTime.UtcNow));
                // Retrieve the roles for the user
                var userRoles = await _userManager.GetRolesAsync(user);

                return GenerateJwtToken(user, userRoles);
            }

            return null;
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