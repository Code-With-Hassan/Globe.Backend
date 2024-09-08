using Globe.Domain.Core.Data;
using Globe.Shared.Models.ResponseDTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Globe.Account.Service.Services.UserService.Impl
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
                return new(await _dbContext.Users.ToListAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw ex;
            }
        }
    }
}
