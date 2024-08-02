using Globe.Shared.Models.ResponseDTOs;

namespace Globe.Account.Service.Services.AuthService
{
    public interface IAuthService
    {
        Task<LoginDTO> LoginAsync(string username, string password);
    }
}
