using Globe.Account.Service.Services.PrivilegesService;
using Globe.Core.Repository;
using Globe.Core.Repository.impl;
using Globe.Domain.Core.Data;
using Globe.EventBus.RabbitMQ.Extensions;
using Globe.EventBus.RabbitMQ.Sender;
using Globe.Shared.Constants;
using Globe.Shared.Entities;
using Globe.Shared.Helpers;
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
using System.Security.Claims;
using System.Text;

namespace Globe.Account.Service.Services.AuthService.Impl
{
    public class AuthService : BaseService<AuthService>, IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IPrivilegesService _privilegesService;
        private readonly UserManager<UserAuthEntity> _userManager;
        private readonly IRepository<UserEntity> _userRepository;
        private readonly IRepository<OrganizationEntity> _organizationRepository;
        private readonly ISuperUserPrivilegesService _superUserPrivilegesService;
        private readonly IRepository<RoleOrganizationsEntity> _roleOrganizationRepository;

        /// <summary>
        /// Constructor for the AuthService class. It initializes the necessary dependencies such as
        /// logging, event sending, user management, configuration, privileges services, and repositories.
        /// It also sets up an event to trigger after logs are saved to the database, sending those logs to RabbitMQ.
        /// </summary>
        /// <param name="logger">Used to log errors, information, and other messages.</param>
        /// <param name="sender">Used to send events, such as logs, to an external service like RabbitMQ.</param>
        /// <param name="userManager">Manages user entities, including authentication and password validation.</param>
        /// <param name="configuration">Provides access to configuration settings, such as JWT settings.</param>
        /// <param name="dbContext">Database context for performing database operations.</param>
        /// <param name="privilegesService">Handles user privileges, including access control.</param>
        /// <param name="superUserPrivilegesService">Handles privileges for super users.</param>
        public AuthService(ILogger<AuthService> logger,
                            IEventSender sender,
                            UserManager<UserAuthEntity> userManager,
                            IConfiguration configuration,
                            ApplicationDbContext dbContext,
                            IPrivilegesService privilegesService,
                            ISuperUserPrivilegesService superUserPrivilegesService) : base(logger)
        {
            _userManager = userManager;
            _configuration = configuration;
            _privilegesService = privilegesService;
            _superUserPrivilegesService = superUserPrivilegesService;

            // Initializing repositories with a generic repository pattern.
            _userRepository = new GenericRepository<UserEntity>(dbContext);
            _organizationRepository = new GenericRepository<OrganizationEntity>(dbContext);
            _roleOrganizationRepository = new GenericRepository<RoleOrganizationsEntity>(dbContext);

            // Event triggered after saving logs to the database, sending logs to RabbitMQ queue.
            sender.SetAfterSaveEvent<UserEntity>(_userRepository);
            sender.SetAfterSaveEvent<OrganizationEntity>(_organizationRepository);
            sender.SetAfterSaveEvent<RoleOrganizationsEntity>(_roleOrganizationRepository);
        }

        /// <summary>
        /// LoginAsync validates the username and password and generates a JWT token if successful.
        /// </summary>
        /// <param name="username">Username to authenticate.</param>
        /// <param name="password">Password to authenticate.</param>
        /// <returns>Returns a LoginDTO containing user privileges and a JWT token.</returns>
        public async Task<LoginDTO> LoginAsync(string username, string password)
        {
            try
            {
                // Find user by username in the Identity framework.
                var user = await _userManager.FindByNameAsync(username);

                if (user is null)
                {
                    throw new Exception(MsgKeys.UsernameIsIncorrect);  // Throw if the user doesn't exist.
                }
                else
                {
                    // Fetch additional user information from the repository.
                    var userEntity = await _userRepository.Query(x => x.Id == user.UserId)
                                                .Include(x => x.UserRoles)
                                                .AsSplitQuery()
                                                .FirstOrDefaultAsync();

                    if (userEntity is null)
                        throw new Exception(MsgKeys.UsernameIsIncorrect);  // Throw if the user entity doesn't exist.

                    user.User = userEntity;

                    // Check if the password matches.
                    if (user != null && await _userManager.CheckPasswordAsync(user, password))
                    {
                        // Update the last login time for the user.
                        await _userRepository.Query(x => x.Id == user.UserId)
                                        .ExecuteUpdateAsync(usr => usr.SetProperty(p => p.LastLoggedIn, DateTimeOffset.UtcNow.ToUnixTimeSeconds()));

                        List<long> organizationIds = new();  // Store organization IDs.
                        UserReadPrivilegesModel userPrivileges = new UserReadPrivilegesModel();

                        // Check if the user is a Super User and assign privileges accordingly.
                        if (user.User.IsSuperUser)
                        {
                            userPrivileges = await _superUserPrivilegesService.GetSuperUserPrivilegesAsync(user.UserId);

                            // Get all company IDs for Super Users.
                            organizationIds = await GetAllCompaniesIds();
                        }
                        else
                        {
                            userPrivileges = await _privilegesService.GetUserPrivilegesAsync(user.UserId);

                            // Get associated company IDs for regular users.
                            organizationIds = await GetAssociatedCompaniesId(user.User.UserRoles.Select(y => y.RoleId).ToList());
                        }

                        // Return the login DTO with user privileges and JWT token.
                        return new LoginDTO
                        {
                            User = userPrivileges,
                            Token = await GenerateJwtToken(user, organizationIds,
                                                            userPrivileges.GetAllowedScreensList(),
                                                            userPrivileges.AllowedApplications)
                        };
                    }
                    throw new Exception("Invalid username or password");  // Throw if login credentials are incorrect.
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);  // Log the error.
                throw ex;  // Rethrow the exception.
            }
        }

        /// <summary>
        /// GetAssociatedCompaniesId returns the list of associated company IDs for the given roles.
        /// </summary>
        /// <param name="roles">List of role IDs.</param>
        /// <returns>Returns a list of associated company IDs.</returns>
        private async Task<List<long>> GetAssociatedCompaniesId(List<long> roles) =>
            await _roleOrganizationRepository
                              .Query(x => roles.Contains(x.RoleId))
                              .Select(x => x.OrganizationId)
                              .ToListAsync();

        /// <summary>
        /// GetAllCompaniesIds returns a list of all company IDs.
        /// </summary>
        /// <returns>Returns a list of all company IDs.</returns>
        private async Task<List<long>> GetAllCompaniesIds() =>
            await _organizationRepository.Query()
                                         .OrderBy(x => x.Id)
                                         .Select(x => x.Id)
                                         .ToListAsync();

        /// <summary>
        /// GenerateJwtToken creates a JWT token for the authenticated user.
        /// </summary>
        /// <param name="user">Authenticated user entity.</param>
        /// <param name="organizationIds">List of organization IDs.</param>
        /// <param name="allowedScreensList">List of allowed screens for the user.</param>
        /// <param name="allowedApplicationsList">List of allowed applications for the user.</param>
        /// <returns>Returns a JWT token as a string.</returns>
        private async Task<string> GenerateJwtToken(UserAuthEntity user, List<long> organizationIds,
                                                    List<string> allowedScreensList, List<string> allowedApplicationsList)
        {
            return await Task.Run(() =>
            {
                // Retrieve JWT settings from the configuration file.
                IConfigurationSection jwtSettings = _configuration.GetSection("Jwt");
                SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings["Key"]));
                SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                // Add claims to the JWT token.
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

                // Create the JWT token with claims and signing credentials.
                JwtSecurityToken token = new JwtSecurityToken(
                    notBefore: DateTime.UtcNow,
                    issuer: jwtSettings["Issuer"],
                    audience: jwtSettings["Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["ExpireMinutes"])),
                    signingCredentials: creds
                );

                // Return the generated token as a string.
                return new JwtSecurityTokenHandler().WriteToken(token);
            });
        }
    }
}
