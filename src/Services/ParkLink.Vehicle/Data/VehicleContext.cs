using MassTransit;
using Microsoft.EntityFrameworkCore;
using ParkLink.Vehicle.Models;

namespace ParkLink.Vehicle.Data
{
    public class VehicleContext : DbContext
    {
        public VehicleContext(DbContextOptions<VehicleContext> options) : base(options)
        {
        }

        public DbSet<Models.Vehicle> Vehicles => Set<Models.Vehicle>();
        public DbSet<VehicleDocument> VehicleDocuments => Set<VehicleDocument>();
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureVehicle(modelBuilder);
            ConfigureVehicleDocument(modelBuilder);

            // MassTransit Transactional Outbox
            modelBuilder.AddOutboxMessageEntity();
            modelBuilder.AddOutboxStateEntity();
            modelBuilder.AddInboxStateEntity();
        }

        private void ConfigureVehicleDocument(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<VehicleDocument>();

            entity.ToTable("VehicleDocuments");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.DocumentType)
                .HasConversion<string>()
                .HasMaxLength(100);

            entity.Property(x => x.DocumentNumber)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.IssuingCountryCode)
                .IsRequired()
                .HasMaxLength(10);

            entity.Property(x => x.DocumentUrl)
                .HasMaxLength(1000);

            entity.HasIndex(x => x.VehicleId);

            entity.HasOne(x => x.Vehicle)
                .WithMany(x => x.Documents)
                .HasForeignKey(x => x.VehicleId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private void ConfigureVehicle(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<Models.Vehicle>();

            entity.ToTable("Vehicles");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.OwnerId)
                .IsRequired()
                .HasMaxLength(450);

            entity.Property(x => x.LicensePlateNumber)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(x => x.VIN)
                .HasMaxLength(100);

            entity.Property(x => x.Make)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.Model)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.Color)
                .HasMaxLength(50);

            entity.Property(x => x.StatusReason)
                    .HasMaxLength(500);

            entity.Property(x => x.VehicleType)
                .HasConversion<string>()
                .HasMaxLength(50);

            entity.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(50);

            entity.Property(x => x.CreatedAtUtc)
                .IsRequired();

            entity.HasIndex(x => x.OwnerId);

            entity.HasIndex(x => x.LicensePlateNumber)
                .IsUnique();

            entity.HasIndex(x => x.VIN)
                .IsUnique()
                .HasFilter("[VIN] IS NOT NULL");

            // Useful for searching active vehicles.
            entity.HasIndex(x => new
            {
                x.OwnerId,
                x.IsActive
            });

            entity.Property(x => x.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
        }
    }
}