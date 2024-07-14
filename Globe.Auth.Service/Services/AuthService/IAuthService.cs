using Globe.Shared.Models.ResponseDTOs;

namespace Globe.Auth.Service.Services.AuthService
{
    public interface IAuthService
    {
        Task<LoginDTO> LoginAsync(string username, string password);
    }
}
