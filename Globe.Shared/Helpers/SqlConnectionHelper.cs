using Globe.Shared.Models;
using System.Data.SqlClient;

namespace Globe.Shared.Helpers
{
    public static class SqlConnectionHelper
    {
        public static string ToConnectionString(SqlConnectionConfiguration config)
        {
            // Create a new SqlConnectionStringBuilder and
            // initialize it with a few name/value pairs.
            var builder = new SqlConnectionStringBuilder();

            builder.Add("Data Source", config.Server);
            builder.Add("Initial Catalog", config.InitialCatalog);
            builder.Add("Encrypt", config.Encrypt);

            if (config.IntegratedSecurity)
            {
                builder.Add("Integrated Security", config.IntegratedSecurity);
            }
            else
            {
                builder.Add("User Id", config.User);
                var decryptedPassword = EncryptionHelper.DecryptString(config.Password);
                builder.Add("Password", decryptedPassword);
            }

            return builder.ConnectionString;
        }
    }
}
