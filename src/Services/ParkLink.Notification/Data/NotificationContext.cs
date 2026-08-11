using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace ParkLink.Notification.Data
{
    public sealed class NotificationContext : DbContext
    {
        public NotificationContext(DbContextOptions<NotificationContext> options) : base(options)
        {
        }

        public DbSet<Models.Notification> Notifications => Set<Models.Notification>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureNotification(modelBuilder);

            // MassTransit transactional inbox/outbox.
            modelBuilder.AddInboxStateEntity();
            modelBuilder.AddOutboxMessageEntity();
            modelBuilder.AddOutboxStateEntity();
        }

        private static void ConfigureNotification(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<Models.Notification>();

            entity.ToTable("Notifications");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.EventType)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(x => x.UserId)
                .IsRequired()
                .HasMaxLength(450);

            entity.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(250);

            entity.Property(x => x.Message)
                .IsRequired()
                .HasMaxLength(4000);

            entity.Property(x => x.ActionUrl)
                .HasMaxLength(1000);

            entity.Property(x => x.Type)
                .HasConversion<string>()
                .HasMaxLength(50);

            entity.Property(x => x.Priority)
                .HasConversion<string>()
                .HasMaxLength(50);

            entity.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(50);

            entity.Property(x => x.Error)
                .HasMaxLength(2000);

            entity.Property(x => x.CorrelationId)
                .HasMaxLength(100);

            /*
             * EventId is the primary idempotency key.
             *
             * The same integration event must not create
             * duplicate notifications for the same user.
             */
            entity.HasIndex(x => new
            {
                x.EventId,
                x.UserId
            })
            .IsUnique();

            entity.HasIndex(x => x.UserId);

            entity.HasIndex(x => new
            {
                x.UserId,
                x.IsRead,
                x.CreatedAtUtc
            });

            entity.HasIndex(x => x.EntityId);

            entity.HasIndex(x => new
            {
                x.Status,
                x.CreatedAtUtc
            });
        }
    }
}
