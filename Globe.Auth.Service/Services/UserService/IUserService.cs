using Globe.Shared.Models.ResponseDTOs;

namespace Globe.Account.Service.Services.UserService
{
    public interface IUserService
    {
        Task<UsersListResponse> GetAllUsers();
    }
}
