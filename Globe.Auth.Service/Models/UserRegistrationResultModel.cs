using Globe.Shared.Entities;
using Microsoft.AspNetCore.Identity;

namespace Globe.Account.Service.Models
{
    /// <summary>
    /// The user registration result.
    /// </summary>
    public class UserRegistrationResultModel
    {
        /// <summary>
        /// Gets or sets the identity result.
        /// </summary>
        public IdentityResult IdentityResult { get; set; }

        /// <summary>
        /// Gets or sets the created user.
        /// </summary>
        public UserEntity User { get; set; }

        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        public Exception Error { get; set; }
    }
}
