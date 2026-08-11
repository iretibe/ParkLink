using MassTransit;
using Microsoft.EntityFrameworkCore;
using ParkLink.Reservation.Models;

namespace ParkLink.Reservation.Data
{
    public class ReservationContext : DbContext
    {
        public ReservationContext(DbContextOptions<ReservationContext> options) : base(options)
        {
        }

        public DbSet<Models.Reservation> Reservations => Set<Models.Reservation>();
        public DbSet<ReservationHold> ReservationHolds => Set<ReservationHold>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureReservation(modelBuilder);

            ConfigureReservationHold(modelBuilder);

            ConfigureMassTransitOutbox(modelBuilder);
        }

        private static void ConfigureReservation(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<Models.Reservation>();

            entity.ToTable("Reservations");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.ReservationNumber)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasIndex(x => x.ReservationNumber)
                .IsUnique();

            entity.Property(x => x.UserId)
                .IsRequired()
                .HasMaxLength(450);

            entity.Property(x => x.ParkingLotName)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(x => x.ParkingSlotNumber)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(x => x.CurrencyCode)
                .IsRequired()
                .HasMaxLength(3);

            entity.Property(x => x.Amount)
                .HasPrecision(18, 2);

            entity.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(50);

            entity.Property(x => x.PaymentStatus)
                .HasConversion<string>()
                .HasMaxLength(50);

            entity.Property(x => x.ReservationType)
                .HasConversion<string>()
                .HasMaxLength(50);

            entity.Property(x => x.AccessMethod)
                .HasConversion<string>()
                .HasMaxLength(50);

            entity.Property(x => x.PaymentReference)
                .HasMaxLength(100);

            entity.Property(x => x.CancellationReason)
                .HasMaxLength(500);

            entity.Property(x => x.AccessCredential)
                .HasMaxLength(500);

            entity.Property(x => x.RowVersion)
                .IsRowVersion();

            entity.HasOne(x => x.Hold)
                .WithOne(x => x.Reservation)
                .HasForeignKey<ReservationHold>(x => x.ReservationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            entity.HasIndex(x => x.UserId);

            entity.HasIndex(x => x.VehicleId);

            entity.HasIndex(x => x.ParkingLotId);

            entity.HasIndex(x => x.ParkingZoneId);

            entity.HasIndex(x => x.ParkingSlotId);

            entity.HasIndex(x => new
            {
                x.UserId,
                x.Status
            });

            entity.HasIndex(x => new
            {
                x.UserId,
                x.CreatedAtUtc
            });

            entity.HasIndex(x => new
            {
                x.ParkingLotId,
                x.Status
            });

            entity.HasIndex(x => new
            {
                x.ParkingSlotId,
                x.Status
            });

            entity.HasIndex(x => new
            {
                x.StartTimeUtc,
                x.EndTimeUtc
            });

            entity.HasIndex(x => new
            {
                x.ParkingSlotId,
                x.StartTimeUtc,
                x.EndTimeUtc,
                x.Status
            });

            entity.HasIndex(x => new
            {
                x.PaymentStatus,
                x.Status
            });

            entity.HasIndex(x => new
            {
                x.Status,
                x.CreatedAtUtc
            });
        }

        private static void ConfigureReservationHold(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<ReservationHold>();

            entity.ToTable("ReservationHolds");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.UserId)
                .IsRequired()
                .HasMaxLength(450);

            entity.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(50);

            entity.HasIndex(x => x.ReservationId)
                .IsUnique();

            entity.HasIndex(x => x.ParkingSlotId);

            entity.HasIndex(x => new
            {
                x.Status,
                x.ExpiresAtUtc
            });

            entity.HasIndex(x => new
            {
                x.ParkingSlotId,
                x.Status,
                x.ExpiresAtUtc
            });
        }

        private static void ConfigureMassTransitOutbox(ModelBuilder modelBuilder)
        {
            modelBuilder.AddInboxStateEntity();
            modelBuilder.AddOutboxMessageEntity();
            modelBuilder.AddOutboxStateEntity();
        }
    }
}
