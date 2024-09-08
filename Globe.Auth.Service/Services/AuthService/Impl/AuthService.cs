using Globe.Core.AuditHelpers;
using Globe.Core.Repository;
using Globe.Core.Repository.impl;
using Globe.Domain.Core.Data;
using Globe.EventBus.RabbitMQ.Event;
using Globe.EventBus.RabbitMQ.Sender;
using Globe.Shared.Constants;
using Globe.Shared.Entities;
using Globe.Shared.Models.ResponseDTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace Globe.Account.Service.Services.AuthService.Impl
{
    public class AuthService : IAuthService
    {
        private readonly ILogger<AuthService> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<UserAuthEntity> _userManager;
        private readonly IRepository<UserEntity> _userRepository;
        private readonly IRepository<RoleEntity> _roleRepository;
        private readonly IRepository<ApplicationEntity> _applicationRepository;
        private readonly IRepository<RoleScreenEntity> _roleScreenRepository;
        private readonly IConfiguration _configuration;

        public AuthService(ILogger<AuthService> logger,
                            IEventSender sender,
                            UserManager<UserAuthEntity> userManager,
                            IConfiguration configuration,
                            ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
            _userManager = userManager;
            _configuration = configuration;
            _userRepository = new GenericRepository<UserEntity>(dbContext);
            _roleRepository = new GenericRepository<RoleEntity>(dbContext);
            _applicationRepository = new GenericRepository<ApplicationEntity>(dbContext);
            _roleScreenRepository = new GenericRepository<RoleScreenEntity>(dbContext);

            ((GenericRepository<UserEntity>)_roleRepository).AfterSave =
            ((GenericRepository<UserEntity>)_userRepository).AfterSave =
            ((GenericRepository<UserEntity>)_applicationRepository).AfterSave =
            ((GenericRepository<UserEntity>)_roleScreenRepository).AfterSave =
                (logs) => sender.SendEvent(new MQEvent<List<AuditEntry>>(RabbitMqQueuesConstants.AuditQueueName, (List<AuditEntry>)logs));

        }

        public async Task<LoginDTO> LoginAsync(string username, string password)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(username);
                user.User = await _userRepository.Query(x => x.Id == user.UserId)
                                                .Include(x => x.UserRoles)
                                                .AsSplitQuery()
                                                .FirstOrDefaultAsync();

                if (user is null)
                    throw new Exception("Invalid username");

                if (user != null && await _userManager.CheckPasswordAsync(user, password))
                {
                    await _dbContext.Users.Where(x => x.Id == user.UserId)
                                    .ExecuteUpdateAsync(usr => usr.SetProperty(p => p.LastLoggedIn, DateTimeOffset.UtcNow.ToUnixTimeSeconds()));

                    ApplicationEntity defaultApplication = new();
                    List<RoleScreenEntity> roleScreenEntries = new();
                    List<ApplicationEntity> allowedApplications = new();
                    
                    if (user.User.IsSuperUser)
                    {
                        var roles = await _roleRepository.Query(x => user.User.UserRoles.Select(y => y.RoleId).Contains(x.Id))
                                                            .Include(x => x.RoleScreens)
                                                            .Include(x => x.DefaultApplication)
                                                            .ToListAsync();

                        roleScreenEntries = await _roleScreenRepository.Query().ToListAsync();
                        allowedApplications = await _applicationRepository.Query().ToListAsync();


                        defaultApplication = roles.FirstOrDefault().DefaultApplication;
                    }
                    else
                    {
                        var roles = await _roleRepository.Query(x => user.User.UserRoles.Select(y => y.RoleId).Contains(x.Id))
                                                            .Include(x => x.RoleApplications)
                                                                .ThenInclude(x => x.Application)
                                                            .Include(x => x.RoleScreens)
                                                            .Include(x => x.DefaultApplication)
                                                            .ToListAsync();

                    }

                    LoginDTO response = new LoginDTO
                    {
                        AllowedScreen = roleScreenEntries,
                        AllowedApplications = allowedApplications,
                        DefaultApplication = defaultApplication,
                        User = user.User,
                        Token = await GenerateJwtToken(user)
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

        private async Task<string> GenerateJwtToken(UserAuthEntity user)
        {
            return await Task.Run(() =>
            {
                IConfigurationSection jwtSettings = _configuration.GetSection("Jwt");
                SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings["Key"]));
                SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                List<Claim> claims = new List<Claim>()
                {
                    new Claim(IAuthConstants.UserId,user.Id.ToString()),
                    new Claim(IAuthConstants.IsSuperUser, user.User.IsSuperUser.ToString()),
                    //new Claim(IAuthConstants.OrganizationIds, JsonConvert.SerializeObject(organizationIds).ToString()),
                    new Claim(IAuthConstants.UserName, user.UserName),
                    //new Claim(IAuthConstants.Privileges, JsonConvert.SerializeObject(allowedScreensList)),
                    //new Claim(IAuthConstants.Applications, JsonConvert.SerializeObject(allowedApplicationsList)),
                    new Claim(IAuthConstants.Scope, IAuthConstants.System),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                JwtSecurityToken token = new JwtSecurityToken(
                    notBefore: DateTime.UtcNow,
                    issuer: jwtSettings["Issuer"],
                    audience: jwtSettings["Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["ExpireMinutes"])),
                    signingCredentials: creds
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            });
        }
    }
}