using Globe.Auth.Service.Data;
using Globe.Shared.Models.ResponseDTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Globe.Auth.Service.Services.UserService.Impl
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly ApplicationDbContext _dbContext;

        public UserService(ILogger<UserService> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<UsersListResponse> GetAllUsers()
        {
            try
            {
                UsersListResponse usersListResponse = new()
                {
                    ApplicationUsers = await _dbContext.ApplicationUser
                                                                    .OrderByDescending(x => x.CreateOn)
                                                                    .Include(x => x.User)
                                                                    .Include(x => x.UserOrganizations)
                                                                    .ToListAsync()
                };
                return usersListResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw ex;
            }
        }
    }
}
