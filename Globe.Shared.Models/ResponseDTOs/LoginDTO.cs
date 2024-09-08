using Globe.Shared.Models.Privileges;

namespace Globe.Shared.Models.ResponseDTOs
{
    public class LoginDTO
    {
        public UserReadPrivilegesModel User { get; set; }

        public string Token { get; set; }
    }
}
