using MassTransit;
using Microsoft.EntityFrameworkCore;
using ParkLink.Gate.Entities;

namespace ParkLink.Gate.Data
{
    public sealed class GateContext : DbContext
    {
        public GateContext(DbContextOptions<GateContext> options) : base(options) { }

        public DbSet<Entities.Gate> Gates => Set<Entities.Gate>();
        public DbSet<GateDevice> GateDevices => Set<GateDevice>();
        public DbSet<RfidTag> RfidTags => Set<RfidTag>();
        public DbSet<OcrRecognition> OcrRecognitions => Set<OcrRecognition>();
        public DbSet<GateAccessAttempt> GateAccessAttempts => Set<GateAccessAttempt>();
        public DbSet<GateDeviceCommand> GateDeviceCommands => Set<GateDeviceCommand>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureGate(modelBuilder);
            ConfigureGateDevice(modelBuilder);
            ConfigureRfidTag(modelBuilder);
            ConfigureOcrRecognition(modelBuilder);
            ConfigureGateAccessAttempt(modelBuilder);
            ConfigureGateDeviceCommand(modelBuilder);

            // MassTransit EF Core Outbox / Inbox
            modelBuilder.AddInboxStateEntity();
            modelBuilder.AddOutboxMessageEntity();
            modelBuilder.AddOutboxStateEntity();
        }

        private void ConfigureGate(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<Entities.Gate>();

            entity.ToTable("Gates");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(x => x.Description)
                .HasMaxLength(500);

            entity.Property(x => x.Type)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(x => x.CreatedAtUtc)
                .IsRequired();

            entity.HasIndex(x => x.ParkingLotId);

            entity.HasIndex(x => new
            {
                x.ParkingLotId,
                x.Name
            })
            .IsUnique();

            entity.HasMany(x => x.Devices)
                .WithOne(x => x.Gate)
                .HasForeignKey(x => x.GateId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(x => x.AccessAttempts)
                .WithOne()
                .HasForeignKey(x => x.GateId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(x => x.RowVersion)
                .IsRowVersion();
        }

        private void ConfigureGateDevice(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<GateDevice>();

            entity.ToTable("GateDevices");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.DeviceName)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(x => x.DeviceIdentifier)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(x => x.Type)
                .HasConversion<string>()
                .HasMaxLength(40)
                .IsRequired();

            entity.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(x => x.IpAddress)
                .HasMaxLength(100);

            entity.Property(x => x.Manufacturer)
                .HasMaxLength(100);

            entity.Property(x => x.Model)
                .HasMaxLength(100);

            entity.HasIndex(x => x.DeviceIdentifier)
                .IsUnique();

            entity.HasIndex(x => x.GateId);

            entity.HasIndex(x => new
            {
                x.GateId,
                x.Type
            });

            entity.Property(x => x.RowVersion)
                .IsRowVersion();
        }

        private void ConfigureRfidTag(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<RfidTag>();

            entity.ToTable("RfidTags");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.TagIdentifier)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(x => x.UserId)
                .HasMaxLength(450);

            entity.HasIndex(x => x.TagIdentifier)
                .IsUnique();

            entity.HasIndex(x => x.VehicleId);

            entity.HasIndex(x => x.UserId);

            entity.HasIndex(x => new
            {
                x.VehicleId,
                x.IsActive
            });
        }

        private void ConfigureOcrRecognition(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<OcrRecognition>();

            entity.ToTable("OcrRecognitions");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.LicensePlate)
                .IsRequired()
                .HasMaxLength(30);

            entity.Property(x => x.Confidence)
                .HasPrecision(5, 4);

            entity.Property(x => x.ImageReference)
                .HasMaxLength(500);

            entity.HasIndex(x => x.GateId);

            entity.HasIndex(x => x.DeviceId);

            entity.HasIndex(x => x.LicensePlate);

            entity.HasIndex(x => x.RecognizedAtUtc);
        }

        private void ConfigureGateAccessAttempt(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<GateAccessAttempt>();

            entity.ToTable("GateAccessAttempts");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.LicensePlate)
                .HasMaxLength(30);

            entity.Property(x => x.RfidTagIdentifier)
                .HasMaxLength(200);

            entity.Property(x => x.Method)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(x => x.Decision)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(x => x.DecisionReason)
                .HasMaxLength(500);

            entity.Property(x => x.UserId)
                .HasMaxLength(450);

            entity.HasIndex(x => x.GateId);

            entity.HasIndex(x => x.VehicleId);

            entity.HasIndex(x => x.ReservationId);

            entity.HasIndex(x => x.LicensePlate);

            entity.HasIndex(x => x.RfidTagIdentifier);

            entity.HasIndex(x => x.DetectedAtUtc);

            entity.HasIndex(x => new
            {
                x.GateId,
                x.DetectedAtUtc
            });
        }

        private static void ConfigureGateDeviceCommand(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<GateDeviceCommand>();

            entity.ToTable("GateDeviceCommands");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Command)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.ErrorMessage)
                .HasMaxLength(500);

            entity.HasIndex(x => x.GateId);

            entity.HasIndex(x => x.DeviceId);

            entity.HasIndex(x => x.AccessAttemptId);

            entity.HasIndex(x => x.RequestedAtUtc);
        }
    }
}
