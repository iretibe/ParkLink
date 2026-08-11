using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ParkLink.Identity.Models;

namespace ParkLink.Identity.Data
{
    public class ApplicationDbContext 
        : IdentityDbContext<ApplicationUser>, IDataProtectionKeyContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = null!;
        public DbSet<UserDocument> UserDocuments => Set<UserDocument>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(x => x.FirstName)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.LastName)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.MiddleName)
                    .HasMaxLength(100);

                entity.Property(x => x.PreferredLanguage)
                    .HasMaxLength(10)
                    .IsRequired();

                entity.Property(x => x.CountryCode)
                    .HasMaxLength(2)
                    .IsRequired();

                entity.Property(x => x.TimeZoneId)
                    .HasMaxLength(100)
                    .IsRequired();
            });

            builder.Entity<UserDocument>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.DocumentNumber)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.IssuingCountryCode)
                    .HasMaxLength(2)
                    .IsRequired();

                entity.HasIndex(x => new
                {
                    x.IssuingCountryCode,
                    x.DocumentType,
                    x.DocumentNumber
                })
                .IsUnique();

                entity.HasOne(x => x.User)
                    .WithMany(x => x.Documents)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
