using MassTransit;
using ParkLink.Reservation.Data;
using ParkLink.Reservation.Messaging.Consumers;

namespace ParkLink.Reservation.Extensions
{
    public static class MessagingExtensions
    {
        public static IServiceCollection AddReservationMessaging(
            this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumer<ParkingSlotStatusChangedConsumer>();
                x.AddConsumer<PaymentAuthorizedConsumer>();
                x.AddConsumer<PaymentCompletedConsumer>();
                x.AddConsumer<PaymentFailedConsumer>();
                x.AddConsumer<PaymentRefundedConsumer>();
                x.AddConsumer<VehicleDeletedConsumer>();
                x.AddConsumer<VehicleEnteredParkingLotConsumer>();
                x.AddConsumer<VehicleExitedParkingLotConsumer>();
                x.AddConsumer<VehicleStatusChangedConsumer>();

                x.AddEntityFrameworkOutbox<ReservationContext>(o =>
                {
                    o.UseSqlServer();
                    o.UseBusOutbox();
                });

                x.UsingRabbitMq((context, cfg) =>
                {
                    var connectionString =
                        configuration.GetConnectionString("rabbitmq");

                    if (string.IsNullOrWhiteSpace(connectionString))
                    {
                        throw new InvalidOperationException(
                            "RabbitMQ connection string 'rabbitmq' was not found.");
                    }

                    cfg.Host(connectionString);

                    cfg.ReceiveEndpoint(
                        "parklink-reservation-parking-events", 
                        e =>
                        {
                            e.ConfigureConsumer<ParkingSlotStatusChangedConsumer>(context);

                            e.UseMessageRetry(r =>
                            {
                                r.Exponential(
                                    5,
                                    TimeSpan.FromSeconds(1),
                                    TimeSpan.FromMinutes(1),
                                    TimeSpan.FromSeconds(5)
                                );
                            });
                        }
                    );

                    cfg.ReceiveEndpoint(
                        "parklink-reservation-payment-events",
                        e =>
                        {
                            e.ConfigureConsumer<PaymentAuthorizedConsumer>(context);
                            e.ConfigureConsumer<PaymentCompletedConsumer>(context);
                            e.ConfigureConsumer<PaymentFailedConsumer>(context);
                            e.ConfigureConsumer<PaymentRefundedConsumer>(context);

                            e.UseMessageRetry(r =>
                            {
                                r.Exponential(
                                    5,
                                    TimeSpan.FromSeconds(1),
                                    TimeSpan.FromMinutes(1),
                                    TimeSpan.FromSeconds(5)
                                );
                            });
                        }
                    );

                    cfg.ReceiveEndpoint(
                        "parklink-reservation-vehicle-events",
                        e =>
                        {
                            e.ConfigureConsumer<VehicleDeletedConsumer>(context);
                            e.ConfigureConsumer<VehicleEnteredParkingLotConsumer>(context);
                            e.ConfigureConsumer<VehicleExitedParkingLotConsumer>(context);
                            e.ConfigureConsumer<VehicleStatusChangedConsumer>(context);

                            e.UseMessageRetry(r =>
                            {
                                r.Exponential(
                                    5,
                                    TimeSpan.FromSeconds(1),
                                    TimeSpan.FromMinutes(1),
                                    TimeSpan.FromSeconds(5)
                                );
                            });
                        }
                    );
                });
            });

            return services;
        }
    }
}
