using Microsoft.AspNetCore.Identity;

namespace Globe.Shared.Entities
{
    public class UserAuthEntity : IdentityUser
    {
        /// <summary>
         /// Gets or sets the user id.
         /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        public UserEntity User { get; set; }
    }
}
