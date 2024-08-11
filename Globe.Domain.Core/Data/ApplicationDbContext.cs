using Globe.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DataEncryption;

namespace Globe.Domain.Core.Data
{
    public class ApplicationDbContext : IdentityDbContext<UserAuthEntity>
    {
        private readonly IEncryptionProvider _provider;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IEncryptionProvider provider)
            : base(options)
        {
            _provider = provider;
            ChangeTracker.LazyLoadingEnabled = false;
        }

        /// <summary>
        /// Gets or sets the users.
        /// </summary>
        new public DbSet<UserEntity> Users { get; set; }

        /// <summary>
        /// Gets or sets the user auth.
        /// </summary>
        //public DbSet<UserAuthEntity> UserAuth { get; set; }
    }

}
