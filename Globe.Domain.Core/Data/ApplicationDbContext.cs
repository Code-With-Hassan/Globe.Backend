using Globe.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DataEncryption;

namespace Globe.Domain.Core.Data
{
    public class ApplicationDbContext : IdentityDbContext<UserAuthEntity>
    {
        private readonly IEncryptionProvider _provider;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IEncryptionProvider provider) : base(options)
        {
            _provider = provider;
            ChangeTracker.LazyLoadingEnabled = false;
        }

        /// <summary>
        /// Gets or sets the users.
        /// </summary>
        new public DbSet<UserEntity> Users { get; set; }

        /// <summary>
        /// Gets or sets the user organizations.
        /// </summary>
        public DbSet<OrganizationEntity> Organizations { get; set; }

        /// <summary>
        /// Gets or sets the applications
        /// </summary>
        public DbSet<ApplicationEntity> Applications { get; set; }

        /// <summary>
        /// Gets or sets the Roles.
        /// </summary>
        public DbSet<RoleEntity> Roles { get; set; }

        /// <summary>
        /// Gets or sets the Role Screen.
        /// </summary>
        public DbSet<RoleScreenEntity> RoleScreens { get; set; }

        /// <summary>
        /// Gets or sets role organizations
        /// </summary>
        public DbSet<RoleOrganizationsEntity> RoleOrganizations { get; set; }

        /// <summary>
        /// Gets or sets role applications
        /// </summary>
        public DbSet<RoleApplicationEntity> RoleApplications { get; set; }

        /// <summary>
        /// Gets or sets screens
        /// </summary>
        public DbSet<ScreenEntity> Screens { get; set; }

        /// <summary>
        /// Gets or sets user roles
        /// </summary>
        public DbSet<UserRoleEntity> UserRoles { get; set; }

        /// <summary>
        /// Gets or sets Common Addresses
        /// </summary>
        public DbSet<AddressEntity> CommonAddresses { get; set; }
    }
}
