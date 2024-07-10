namespace Globe.Auth.Service.Services.UserRegistrationService
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
        Task<bool> RegisterUserAsync(string username, string email, string password);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<bool> RegisterAdminAsync(string username, string email, string password);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<bool> RegisterSuperAdminAsync(string username, string email, string password);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<bool> RegisterUnverifiedUserAsync(string username, string email, string password);
    }
}
