using Globe.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Globe.Audit.Api.Database
{
    /// <summary>
    /// Database context for Audits and related entities (if any)
    /// </summary>
    public class AuditDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuditDbContext"/> class.
        /// </summary>
        /// <param name="options">DB Context options/configs</param>
        public AuditDbContext(DbContextOptions<AuditDbContext> options)
            : base(options)
        {
            ChangeTracker.LazyLoadingEnabled = false;
            Database.SetCommandTimeout(new TimeSpan(0, 3, 0));
        }

        /// <summary>
        /// Lifecycle method triggers when the model is being created
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            BuildAuditModel(modelBuilder);
        }

        /// <summary>
        /// Builds the Audit model.
        /// </summary>
        /// <param name="modelBuilder">The model builder to be used to create the Audit table schema.</param>
        private void BuildAuditModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuditEntity>(builder =>
            {
                builder.Property(a => a.Id).UseIdentityColumn().IsRequired();
                builder.Property(a => a.AuditDateTimeUtc).IsRequired();
                builder.Property(a => a.AuditType).HasMaxLength(30).IsRequired();
                builder.Property(a => a.AuditUser).HasMaxLength(50).IsRequired();
                builder.Property(a => a.ChangedColumns).IsRequired();
                builder.Property(a => a.KeyValues).IsRequired();
                builder.Property(a => a.NewValues).IsRequired();
                builder.Property(a => a.OldValues).IsRequired();
                builder.Property(a => a.TableName).IsRequired();
                builder.Property(a => a.IsActive).HasDefaultValue(true).IsRequired();
            });

            modelBuilder.Entity<AuditOrganizationEntity>(entity =>
            {
                entity.ToTable("AuditOrganization");
            });

        }

        /// <summary>
        /// Gets or sets the audit.
        /// </summary>
        public DbSet<AuditEntity> Audits { get; set; }

        /// <summary>
        /// Gets or sets the Table.
        /// </summary>AuditUserEntity
        public DbSet<AuditTableEntity> AuditTables { get; set; }

        /// <summary>
        /// Get or Set AuditOrganization
        /// </summary>
        public DbSet<AuditOrganizationEntity> AuditOrganization { get; set; }
    }
}
