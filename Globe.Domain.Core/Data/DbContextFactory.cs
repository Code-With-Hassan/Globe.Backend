using Globe.Core.SqlExecution;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Storage;

namespace Globe.Domain.Core.Data
{
    public class DbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer("Data Source=DESKTOP-8BL3MIG\\SQLEXPRESS;Initial Catalog=Globe;Encrypt=False;Integrated Security=True");

            optionsBuilder.ReplaceService<IRelationalCommandBuilderFactory, DynamicSqlRelationalCommandBuilderFactory>();

            return new ApplicationDbContext(optionsBuilder.Options, null);
        }
    }
}
