using Globe.Account.Service.Data;
using Globe.Account.Service.Services.RoleService;
using Globe.Account.Service.Services.UserRegistrationService;
using Globe.Shared.Entities;
using Globe.Shared.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Globe.Account.Service.Services.UserRegistrationService.Impl
{
    /// <summary>
    /// Service for user registration.
    /// </summary>
    public class UserRegistrationService : IUserRegistrationService
    {
        private readonly IRoleService _roleService;
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<UserRegistrationService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRegistrationService"/> class.
        /// </summary>
        /// <param name="dbContext">Database context.</param>
        /// <param name="userManager">User manager.</param>
        /// <param name="roleService">Role service.</param>
        /// <param name="logger">Logger service.</param>
        public UserRegistrationService(ApplicationDbContext dbContext,
                                        IRoleService roleService,
                                        UserManager<IdentityUser> userManager,
                                        ILogger<UserRegistrationService> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
            _userManager = userManager;
            _roleService = roleService;
        }

        /// <summary>
        /// Registers a user with a default role of "User".
        /// </summary>
        /// <param name="username">Username.</param>
        /// <param name="email">Email.</param>
        /// <param name="password">Password.</param>
        /// <returns>True if registration succeeded, otherwise false.</returns>
        public async Task<bool> RegisterUserAsync(string username, string email, string password)
        {
            return await RegisterAsync(username, email, password, EUserRole.User.ToString());
        }

        /// <summary>
        /// Registers an admin user.
        /// </summary>
        /// <param name="username">Username.</param>
        /// <param name="email">Email.</param>
        /// <param name="password">Password.</param>
        /// <returns>True if registration succeeded, otherwise false.</returns>
        public async Task<bool> RegisterAdminAsync(string username, string email, string password)
        {
            return await RegisterAsync(username, email, password, EUserRole.Admin.ToString());
        }

        /// <summary>
        /// Registers a super admin user.
        /// </summary>
        /// <param name="username">Username.</param>
        /// <param name="email">Email.</param>
        /// <param name="password">Password.</param>
        /// <returns>True if registration succeeded, otherwise false.</returns>
        public async Task<bool> RegisterSuperAdminAsync(string username, string email, string password)
        {
            return await RegisterAsync(username, email, password, EUserRole.SuperAdmin.ToString());
        }

        /// <summary>
        /// Registers an unverified user.
        /// </summary>
        /// <param name="username">Username.</param>
        /// <param name="email">Email.</param>
        /// <param name="password">Password.</param>
        /// <returns>True if registration succeeded, otherwise false.</returns>
        public async Task<bool> RegisterUnverifiedUserAsync(string username, string email, string password)
        {
            return await RegisterAsync(username, email, password, EUserRole.UnverifiedUser.ToString());
        }

        /// <summary>
        /// Registers a user with the specified role.
        /// </summary>
        /// <param name="username">Username.</param>
        /// <param name="email">Email.</param>
        /// <param name="password">Password.</param>
        /// <param name="defaultRole">Default role.</param>
        /// <returns>True if registration succeeded, otherwise false.</returns>
        /// <exception cref="ApplicationException">Thrown when an error occurs during registration.</exception>
        private async Task<bool> RegisterAsync(string username, string email, string password, string defaultRole = "User")
        {
            try
            {
                var user = new IdentityUser { UserName = username, Email = email };
                var result = await _userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    await _roleService.EnsureRoleExistsAsync(defaultRole);
                    await _roleService.AssignRoleAsync(user.Id, defaultRole);

                    ApplicationUser applicationUser = new ApplicationUser
                    {
                        CreateOn = DateTime.UtcNow,
                        UserId = user.Id,
                        lastLoggedIn = DateTime.MinValue
                    };

                    await _dbContext.ApplicationUser.AddAsync(applicationUser);
                    await _dbContext.SaveChangesAsync();

                    _logger.LogInformation($"User {username} registered successfully with role {defaultRole}.");
                }

                return result.Succeeded;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during registration.");
                // Log exception
                throw new ApplicationException("An error occurred during registration", ex);
            }
        }
    }
}
