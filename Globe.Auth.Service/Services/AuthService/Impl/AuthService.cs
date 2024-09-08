using Globe.Account.Service.Services.PrivilegesService;
using Globe.Account.Service.Services.PrivilegesService.Impl;
using Globe.Core.AuditHelpers;
using Globe.Core.Repository;
using Globe.Core.Repository.impl;
using Globe.Domain.Core.Data;
using Globe.EventBus.RabbitMQ.Event;
using Globe.EventBus.RabbitMQ.Sender;
using Globe.Shared.Constants;
using Globe.Shared.Entities;
using Globe.Shared.Models.Privileges;
using Globe.Shared.Models.ResponseDTOs;
using Globe.Shared.MVC.Resoures;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
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
        private readonly IRepository<OrganizationEntity> _organizationRepository;
        private readonly IPrivilegesService _privilegesService;
        private readonly ISuperUserPrivilegesService _superUserPrivilegesService;
        private readonly IRepository<ApplicationEntity> _applicationRepository;
        private readonly IRepository<RoleOrganizationsEntity> _roleOrganizationRepository;
        private readonly IConfiguration _configuration;

        public AuthService(ILogger<AuthService> logger,
                            IEventSender sender,
                            UserManager<UserAuthEntity> userManager,
                            IConfiguration configuration,
                            ApplicationDbContext dbContext,
                            IPrivilegesService privilegesService,
                            ISuperUserPrivilegesService superUserPrivilegesService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _userManager = userManager;
            _configuration = configuration;
            _privilegesService = privilegesService;
            _superUserPrivilegesService = superUserPrivilegesService;
            _userRepository = new GenericRepository<UserEntity>(dbContext);
            _applicationRepository = new GenericRepository<ApplicationEntity>(dbContext);
            _organizationRepository = new GenericRepository<OrganizationEntity>(dbContext);
            _roleOrganizationRepository = new GenericRepository<RoleOrganizationsEntity>(dbContext);

            ((GenericRepository<UserEntity>)_userRepository).AfterSave =
            ((GenericRepository<UserEntity>)_applicationRepository).AfterSave =
            ((GenericRepository<UserEntity>)_organizationRepository).AfterSave =
            ((GenericRepository<RoleOrganizationsEntity>)_roleOrganizationRepository).AfterSave =
                (logs) => sender.SendEvent(new MQEvent<List<AuditEntry>>(RabbitMqQueuesConstants.AuditQueueName, (List<AuditEntry>)logs));

        }

        public async Task<LoginDTO> LoginAsync(string username, string password)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(username);

                if (user is null)
                {
                    throw new Exception(MsgKeys.UsernameIsIncorrect);
                }
                else
                {
                    var userEntity = await _userRepository.Query(x => x.Id == user.UserId)
                                                .Include(x => x.UserRoles)
                                                .AsSplitQuery()
                                                .FirstOrDefaultAsync();

                    if (userEntity is null)
                        throw new Exception(MsgKeys.UsernameIsIncorrect);

                    user.User = userEntity;

                    if (user != null && await _userManager.CheckPasswordAsync(user, password))
                    {
                        await _userRepository.Query(x => x.Id == user.UserId)
                                        .ExecuteUpdateAsync(usr => usr.SetProperty(p => p.LastLoggedIn, DateTimeOffset.UtcNow.ToUnixTimeSeconds()));

                        List<long> organizationIds = new();
                        UserReadPrivilegesModel userPrivileges = new UserReadPrivilegesModel();

                        if (user.User.IsSuperUser)
                        {
                            userPrivileges = await _superUserPrivilegesService.GetSuperUserPrivilegesAsync(user.UserId);

                            // get all company ids from auth service
                            organizationIds = await GetAllCompaniesIds();
                        }
                        else
                        {
                            userPrivileges = await _privilegesService.GetUserPrivilegesAsync(user.UserId);

                            // get associated company ids from auth service
                            organizationIds = await GetAssociatedCompaniesId(user.User.UserRoles.Select(y => y.RoleId).ToList());
                        }


                        return new LoginDTO
                        {
                            User = userPrivileges,
                            Token = await GenerateJwtToken(user, organizationIds,
                                                            userPrivileges.GetAllowedScreensList(),
                                                            userPrivileges.AllowedApplications)
                        };
                    }
                    throw new Exception("Invalid username or password");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw ex;
            }
        }

        private async Task<List<long>> GetAssociatedCompaniesId(List<long> roles) =>
            await _roleOrganizationRepository
                              .Query(x => roles.Contains(x.RoleId))
                              .Select(x => x.OrganizationId)
                              .ToListAsync();

        private async Task<List<long>> GetAllCompaniesIds() =>
            await _organizationRepository.Query()
                                         .OrderBy(x => x.Id)
                                         .Select(x => x.Id)
                                         .ToListAsync();

        private async Task<string> GenerateJwtToken(UserAuthEntity user, List<long> organizationIds,
                                                    List<string> allowedScreensList, List<string> allowedApplicationsList)
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
                    new Claim(IAuthConstants.OrganizationIds, JsonConvert.SerializeObject(organizationIds).ToString()),
                    new Claim(IAuthConstants.UserName, user.UserName),
                    new Claim(IAuthConstants.Privileges, JsonConvert.SerializeObject(allowedScreensList)),
                    new Claim(IAuthConstants.Applications, JsonConvert.SerializeObject(allowedApplicationsList)),
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