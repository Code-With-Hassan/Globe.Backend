namespace Globe.Shared.Models
{
    public class SqlConnectionConfiguration
    {
        public string Server { get; set; }
        public string InitialCatalog { get; set; }
        public bool IntegratedSecurity { get; set; }
        public bool Encrypt { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
    }
}
