using AutoMapper;
using Globe.Account.Service.Models;
using Globe.Account.Service.Services.RoleService;
using Globe.Core.AuditHelpers;
using Globe.Core.Repository;
using Globe.Core.Repository.impl;
using Globe.Domain.Core.Data;
using Globe.EventBus.RabbitMQ.Event;
using Globe.EventBus.RabbitMQ.Sender;
using Globe.Shared.Constants;
using Globe.Shared.Entities;
using Globe.Shared.Enums;
using Globe.Shared.Helpers;
using Globe.Shared.MVC.Resoures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Globe.Account.Service.Services.UserRegistrationService.Impl
{
    /// <summary>
    /// Service for user registration.
    /// </summary>
    public class UserRegistrationService : BaseService, IUserRegistrationService
    {
        private readonly IRoleService _roleService;
        private readonly ApplicationDbContext _dbContext;
        private readonly IRepository<UserEntity> _userRepository;
        private readonly UserManager<UserAuthEntity> _userManager;
        private readonly ILogger<UserRegistrationService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRegistrationService"/> class.
        /// </summary>
        /// <param name="dbContext">Database context.</param>
        /// <param name="userManager">User manager.</param>
        /// <param name="roleService">Role service.</param>
        /// <param name="logger">Logger service.</param>
        public UserRegistrationService(IRoleService roleService,
                                        IEventSender sender,
                                        ApplicationDbContext dbContext,
                                        IHttpContextAccessor httpContext,
                                        UserManager<UserAuthEntity> userManager,
                                        ILogger<UserRegistrationService> logger) : base(httpContext)
        {
            _logger = logger;
            _dbContext = dbContext;
            _roleService = roleService;
            _userManager = userManager;
            _userRepository = new GenericRepository<UserEntity>(dbContext);

            ((GenericRepository<UserEntity>)_userRepository).AfterSave =
                (logs) => sender.SendEvent(new MQEvent<List<AuditEntry>>(RabbitMqQueuesConstants.AuditQueueName, (List<AuditEntry>)logs));

        }

        /// <summary>
        /// Registers a user with a default role of "User".
        /// </summary>
        /// <param name="username">Username.</param>
        /// <param name="email">Email.</param>
        /// <param name="password">Password.</param>
        /// <returns>True if registration succeeded, otherwise false.</returns>
        public async Task<UserRegistrationResultModel> RegisterUserAsync(string username, string email, string password)
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
        public async Task<UserRegistrationResultModel> RegisterAdminAsync(string username, string email, string password)
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
        public async Task<UserRegistrationResultModel> RegisterSuperAdminAsync(string username, string email, string password)
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
        public async Task<UserRegistrationResultModel> RegisterUnverifiedUserAsync(string username, string email, string password)
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
        private async Task<UserRegistrationResultModel> RegisterAsync(string username, string email, string password, string defaultRole = "User")
        {
            UserRegistrationResultModel response = new UserRegistrationResultModel() { IdentityResult = IdentityResult.Failed() };
            using (var transaction = _userRepository.GetTransaction())
            {
                try
                {
                    UserEntity userEntity = new UserEntity()
                    {
                        PasswordResetTime = DateTime.UtcNow,
                        Email = email,
                        UserName = username,
                        IsLocked = false,
                        IsSuperUser = false,
                    };

                    _userRepository.Insert(userEntity);
                    _userRepository.SaveChanges(UserName, DefaultOrganizationId, false);

                    response.User = userEntity;

                    // creating empty identity object to be passed to usermanager
                    var identityUser = new UserAuthEntity()
                    {
                        UserName = username,
                        Email = email,
                        UserId = userEntity.Id
                    };

                    var result = await _userManager.CreateAsync(identityUser, password);

                    if (result.Succeeded)
                    {
                        await _roleService.EnsureRoleExistsAsync(defaultRole);
                        await _roleService.AssignRoleAsync(identityUser.Id, defaultRole);

                        // Commit transaction
                        transaction.Commit();

                        _logger.LogInformation($"User {username} registered successfully with role {defaultRole}.");
                    }
                    else
                    {
                        transaction.Rollback();
                    }
                }
                catch (DbUpdateException)
                {
                    response.Error = new Exception(MsgKeys.UserAlreadyExists);
                    transaction.Rollback();
                }
                catch (Exception ex)
                {
                    response.Error = ex;
                    transaction.Rollback();
                }
            }
            return response;
        }

        /// <summary>
        /// Gets the error message dictionary.
        /// </summary>
        /// <param name="error">The error message.</param>
        /// <returns>A Dictionary object.</returns>
        public Dictionary<string, string> GetErrorMessageDictionary(string error)
        {
            return new Dictionary<string, string>
            {
                [error] = error
            };
        }
    }
}
