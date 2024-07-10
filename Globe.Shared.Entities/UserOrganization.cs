using System.ComponentModel.DataAnnotations.Schema;

namespace Globe.Shared.Entities
{
    public class UserOrganization
    {
        public int Id { get; set; }

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public ApplicationUser User { get; set; }

        [ForeignKey(nameof(Organization))]
        public int OrganizationId { get; set; }
        public Organization Organization { get; set; }
    }
}
