using MassTransit;
using ParkLink.Gate.Data;
using ParkLink.Gate.Events;
using ParkLink.Gate.Messaging.Consumers.Payment;
using ParkLink.Gate.Messaging.Consumers.Reservation;
using ParkLink.Gate.Messaging.Consumers.Vehicle;

namespace ParkLink.Gate.Extensions
{
    public static class GateMessagingExtensions
    {
        public static IServiceCollection AddGateMessaging(
            this IServiceCollection services)
        {
            services.AddMassTransit(x =>
            {
                // Vehicle consumers
                x.AddConsumer<VehicleCreatedConsumer>();
                x.AddConsumer<VehicleUpdatedConsumer>();
                x.AddConsumer<VehicleSuspendedConsumer>();
                x.AddConsumer<VehicleDeletedConsumer>();

                // Reservation consumers
                x.AddConsumer<ReservationCreatedConsumer>();
                x.AddConsumer<ReservationCancelledConsumer>();
                x.AddConsumer<ReservationExpiredConsumer>();

                // Payment consumers
                x.AddConsumer<PaymentCompletedConsumer>();
                x.AddConsumer<PaymentFailedConsumer>();
                x.AddConsumer<PaymentRefundedConsumer>();

                // Transactional Outbox
                x.AddEntityFrameworkOutbox<GateContext>(o =>
                {
                    o.UseSqlServer();
                    o.QueryDelay = TimeSpan.FromSeconds(1);
                    o.DuplicateDetectionWindow = TimeSpan.FromMinutes(5);
                    o.UseBusOutbox();
                });

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(
                        "rabbitmq",
                        "/",
                        h =>
                        {
                            h.Username("guest");
                            h.Password("guest");
                        });

                    cfg.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }
}
