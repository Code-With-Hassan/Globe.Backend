using Globe.Shared.Entities;

namespace Globe.Shared.Models.ResponseDTOs
{
    public class UsersListResponse
    {
        public UsersListResponse(List<UserEntity> users)
        {
            Users = users;
        }
        public List<UserEntity> Users { get; set; }
    }
}
