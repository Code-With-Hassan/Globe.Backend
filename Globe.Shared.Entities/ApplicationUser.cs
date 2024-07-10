namespace Globe.Shared.Entities
{
    using Microsoft.AspNetCore.Identity;
    using System.ComponentModel.DataAnnotations.Schema;

    public class ApplicationUser
    {
        public int Id { get; set; }

        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public IdentityUser User { get; set; }
        public DateTime CreateOn { get; set; }
        public DateTime? lastLoggedIn { get; set; }

        public virtual ICollection<UserOrganization> UserOrganizations { get; set; }
    }
}
