using Globe.Shared.Entities;
using Microsoft.AspNetCore.Identity;

namespace Globe.Shared.Models.ResponseDTOs
{
    public class LoginDTO
    {
        public string Token { get; set; }
        public IdentityUser User { get; set; }
        public List<UserOrganization> UserOrganizations { get; set; }
    }
}
