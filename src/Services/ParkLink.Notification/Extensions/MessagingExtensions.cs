using MassTransit;
using ParkLink.Notification.Consumers.Reservation;
using ParkLink.Notification.Consumers.Vehicle;
using ParkLink.Notification.Data;

namespace ParkLink.Notification.Extensions
{
    public static class MessagingExtensions
    {
        public static IServiceCollection AddNotificationMessaging(
            this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumer<VehicleCreatedNotificationConsumer>();
                x.AddConsumer<VehicleUpdatedNotificationConsumer>();
                x.AddConsumer<VehicleDeletedNotificationConsumer>();
                x.AddConsumer<VehicleStatusChangedNotificationConsumer>();

                x.AddConsumer<ReservationCreatedNotificationConsumer>();
                x.AddConsumer<ReservationConfirmedNotificationConsumer>();
                x.AddConsumer<ReservationCancelledNotificationConsumer>();
                x.AddConsumer<ReservationExpiredNotificationConsumer>();
                x.AddConsumer<ReservationNoShowNotificationConsumer>();
                x.AddConsumer<ReservationExtendedNotificationConsumer>();
                x.AddConsumer<ReservationActivatedNotificationConsumer>();
                x.AddConsumer<ReservationCompletedNotificationConsumer>();
                x.AddConsumer<ReservationCheckedInNotificationConsumer>();
                x.AddConsumer<ReservationCheckOutNotificationConsumer>();
                x.AddConsumer<ReservationHoldCreatedNotificationConsumer>();
                x.AddConsumer<ReservationHoldReleasedNotificationConsumer>();
                x.AddConsumer<ReservationPaymentStatusChangedNotificationConsumer>();

                x.AddEntityFrameworkOutbox<NotificationContext>(options =>
                {
                    options.UseSqlServer();
                    options.UseBusOutbox();
                });

                x.UsingRabbitMq((context, cfg) =>
                {
                    var connectionString = configuration.GetConnectionString("rabbitmq");

                    if (string.IsNullOrWhiteSpace(connectionString))
                    {
                        throw new InvalidOperationException(
                            "RabbitMQ connection string 'rabbitmq' was not found.");
                    }

                    cfg.Host(connectionString);

                    cfg.UseMessageRetry(retry =>
                    {
                        retry.Exponential(
                            retryLimit: 5,
                            minInterval: TimeSpan.FromSeconds(1),
                            maxInterval: TimeSpan.FromMinutes(1),
                            intervalDelta: TimeSpan.FromSeconds(5)
                        );
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }
}
