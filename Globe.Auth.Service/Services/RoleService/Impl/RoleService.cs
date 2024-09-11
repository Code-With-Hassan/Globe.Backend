using Globe.Shared.Entities;
using Globe.Shared.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Globe.Account.Service.Services.RoleService.Impl
{
    public class RoleService : BaseService<RoleService>, IRoleService
    {
        private readonly UserManager<UserAuthEntity> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleService"/> class.
        /// </summary>
        /// <param name="userManager">User manager.</param>
        /// <param name="roleManager">Role manager.</param>
        /// <param name="logger">Logger service.</param>
        public RoleService(ILogger<RoleService> logger,
                            UserManager<UserAuthEntity> userManager,
                            RoleManager<IdentityRole> roleManager): base(logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        /// <summary>
        /// Assigns a role to a user.
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <param name="role">Role name.</param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        public async Task AssignRoleAsync(string userId, string role)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    if (!await _roleManager.RoleExistsAsync(role))
                    {
                        await EnsureRoleExistsAsync(role);
                    }
                    await _userManager.AddToRoleAsync(user, role);
                    _logger.LogInformation($"Role {role} assigned to user {userId}.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while assigning role {role} to user {userId}.");
                throw new ApplicationException($"An error occurred while assigning role {role} to user {userId}", ex);
            }
        }

        /// <summary>
        /// Ensures a role exists.
        /// </summary>
        /// <param name="role">Role name.</param>
        public async Task EnsureRoleExistsAsync(string role)
        {
            try
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    var identityRole = new IdentityRole(role);
                    var result = await _roleManager.CreateAsync(identityRole);
                    if (!result.Succeeded)
                    {
                        throw new ApplicationException($"An error occurred while creating role {role}");
                    }
                    _logger.LogInformation($"Role {role} created.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while ensuring role {role} exists.");
                // Log exception
                throw new ApplicationException($"An error occurred while ensuring role {role} exists", ex);
            }
        }
    }
}
