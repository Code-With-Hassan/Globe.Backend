namespace Globe.Shared.Entities
{
    public class Organization
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public virtual ICollection<UserOrganization> UserOrganizations { get; set; }
    }
}
