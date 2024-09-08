using Globe.Shared.Entities;
using Microsoft.AspNetCore.Identity;

namespace Globe.Shared.Models.ResponseDTOs
{
    public class LoginDTO
    {
        public string Token { get; set; }
        public UserEntity User { get; set; }
        public List<ApplicationEntity> AllowedApplications { get; set; }
        public ApplicationEntity DefaultApplication { get; set; }
        public List<RoleScreenEntity> AllowedScreen { get; set; }
    }
}
