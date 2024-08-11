using Globe.Account.Service.Models;

namespace Globe.Account.Service.Services.UserRegistrationService
{
    /// <summary>
    /// 
    /// </summary>
    public interface IUserRegistrationService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<UserRegistrationResultModel> RegisterUserAsync(string username, string email, string password);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<UserRegistrationResultModel> RegisterAdminAsync(string username, string email, string password);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<UserRegistrationResultModel> RegisterSuperAdminAsync(string username, string email, string password);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<UserRegistrationResultModel> RegisterUnverifiedUserAsync(string username, string email, string password);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Dictionary<string,string> GetErrorMessageDictionary(string message);
    }
}
