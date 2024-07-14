using Globe.Shared.Models.ResponseDTOs;

namespace Globe.Auth.Service.Services.UserService
{
    public interface IUserService
    {
        Task<UsersListResponse> GetAllUsers();
    }
}
