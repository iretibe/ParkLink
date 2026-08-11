using MassTransit;
using Microsoft.EntityFrameworkCore;
using ParkLink.Parking.Models;

namespace ParkLink.Parking.Data
{
    public class ParkingContext : DbContext
    {
        public ParkingContext(DbContextOptions<ParkingContext> options) : base(options)
        {
        }

        public DbSet<ParkingLot> ParkingLots => Set<ParkingLot>();
        public DbSet<ParkingZone> ParkingZones => Set<ParkingZone>();
        public DbSet<ParkingSlot> ParkingSlots => Set<ParkingSlot>();
        public DbSet<ParkingGate> ParkingGates => Set<ParkingGate>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureParkingLot(modelBuilder);
            ConfigureParkingZone(modelBuilder);
            ConfigureParkingSlot(modelBuilder);
            ConfigureParkingGate(modelBuilder);

            modelBuilder.AddInboxStateEntity();
            modelBuilder.AddOutboxMessageEntity();
            modelBuilder.AddOutboxStateEntity();
        }

        private static void ConfigureParkingLot(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<ParkingLot>();

            entity.ToTable("ParkingLots");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(x => x.CountryCode)
                .IsRequired()
                .HasMaxLength(10);

            entity.Property(x => x.City)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(50);

            entity.HasIndex(x => x.CountryCode);

            entity.HasIndex(x => new
            {
                x.CountryCode,
                x.City,
                x.IsActive
            });
        }

        private static void ConfigureParkingZone(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<ParkingZone>();

            entity.ToTable("ParkingZones");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(50);

            entity.HasOne(x => x.ParkingLot)
                .WithMany(x => x.Zones)
                .HasForeignKey(x => x.ParkingLotId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => x.ParkingLotId);
        }

        private static void ConfigureParkingSlot(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<ParkingSlot>();

            entity.ToTable("ParkingSlots");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.SlotNumber)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(x => x.SlotType)
                .HasConversion<string>()
                .HasMaxLength(50);

            entity.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(50);

            entity.Property(x => x.RowVersion)
                .IsRowVersion();

            entity.HasOne(x => x.ParkingZone)
                .WithMany(x => x.Slots)
                .HasForeignKey(x => x.ParkingZoneId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => new
            {
                x.ParkingZoneId,
                x.SlotNumber
            })
            .IsUnique();

            entity.HasIndex(x => new
            {
                x.ParkingZoneId,
                x.Status,
                x.IsActive
            });
        }

        private static void ConfigureParkingGate(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<ParkingGate>();

            entity.ToTable("ParkingGates");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.GateType)
                .HasConversion<string>()
                .HasMaxLength(50);

            entity.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(50);

            entity.HasOne(x => x.ParkingLot)
                .WithMany(x => x.Gates)
                .HasForeignKey(x => x.ParkingLotId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => x.ParkingLotId);

            entity.HasIndex(x => x.DeviceIdentifier)
                .IsUnique()
                .HasFilter("[DeviceIdentifier] IS NOT NULL");

            entity.HasIndex(x => x.RfidReaderIdentifier)
                .IsUnique()
                .HasFilter("[RfidReaderIdentifier] IS NOT NULL");

            entity.HasIndex(x => x.OcrCameraIdentifier)
                .IsUnique()
                .HasFilter("[OcrCameraIdentifier] IS NOT NULL");
        }
    }
}
