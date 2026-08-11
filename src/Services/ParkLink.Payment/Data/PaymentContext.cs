using MassTransit;
using Microsoft.EntityFrameworkCore;
using ParkLink.Payment.Models;

namespace ParkLink.Payment.Data
{
    public class PaymentContext : DbContext
    {
        public PaymentContext(DbContextOptions<PaymentContext> options) : base(options)
        {
        }

        public DbSet<Models.Payment> Payments => Set<Models.Payment>();
        public DbSet<PaymentTransaction> PaymentTransactions => Set<PaymentTransaction>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigurePayment(modelBuilder);
            ConfigurePaymentTransaction(modelBuilder);

            modelBuilder.AddInboxStateEntity();
            modelBuilder.AddOutboxMessageEntity();
            modelBuilder.AddOutboxStateEntity();
        }

        private static void ConfigurePayment(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<Models.Payment>();

            entity.ToTable("Payments");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.ReservationNumber)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(x => x.UserId)
                .IsRequired()
                .HasMaxLength(450);

            entity.Property(x => x.CurrencyCode)
                .IsRequired()
                .HasMaxLength(3);

            entity.Property(x => x.Amount)
                .HasPrecision(18, 2);

            entity.Property(x => x.RefundedAmount)
                .HasPrecision(18, 2);

            entity.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(50);

            entity.Property(x => x.Method)
                .HasConversion<string>()
                .HasMaxLength(50);

            entity.Property(x => x.Provider)
                .HasMaxLength(100);

            entity.Property(x => x.ProviderReference)
                .HasMaxLength(200);

            entity.Property(x => x.PaymentReference)
                .HasMaxLength(200);

            entity.Property(x => x.FailureReason)
                .HasMaxLength(500);

            entity.Property(x => x.RowVersion)
                .IsRowVersion();

            entity.HasIndex(x => x.ReservationId);

            entity.HasIndex(x => x.UserId);

            entity.HasIndex(x => x.ProviderReference)
                .IsUnique()
                .HasFilter("[ProviderReference] IS NOT NULL");

            entity.HasIndex(x => x.PaymentReference)
                .IsUnique()
                .HasFilter("[PaymentReference] IS NOT NULL");

            // One active Payment aggregate per reservation.
            entity.HasIndex(x => x.ReservationId)
                .IsUnique();
        }

        private static void ConfigurePaymentTransaction(ModelBuilder modelBuilder)
        {
            var entity =
                modelBuilder.Entity<PaymentTransaction>();

            entity.ToTable("PaymentTransactions");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Type)
                .HasConversion<string>()
                .HasMaxLength(50);

            entity.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(50);

            entity.Property(x => x.Amount)
                .HasPrecision(18, 2);

            entity.Property(x => x.CurrencyCode)
                .HasMaxLength(3)
                .IsRequired();

            entity.HasOne(x => x.Payment)
                .WithMany(x => x.Transactions)
                .HasForeignKey(x => x.PaymentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => x.PaymentId);

            entity.HasIndex(x => x.ProviderReference);
        }
    }
}
