using Globe.Shared.Entities;

namespace Globe.Shared.Models.ResponseDTOs
{
    public class UsersListResponse
    {
        public List<UserEntity> ApplicationUsers { get; set; }
    }
}
