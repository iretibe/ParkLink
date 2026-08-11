using MassTransit;
using ParkLink.Parking.Data;

namespace ParkLink.Parking.Messaging
{
    public static class MessagingExtensions
    {
        public static IServiceCollection AddParkingMessaging(
            this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMassTransit(x =>
            {
                x.AddEntityFrameworkOutbox<ParkingContext>(o =>
                {
                    o.UseSqlServer();

                    o.UseBusOutbox();
                });

                x.UsingRabbitMq((context, cfg) =>
                {
                    var rabbitMqConnectionString =
                        configuration.GetConnectionString("rabbitmq");

                    if (string.IsNullOrWhiteSpace(
                        rabbitMqConnectionString))
                    {
                        throw new InvalidOperationException(
                            "RabbitMQ connection string " +
                            "'rabbitmq' was not found.");
                    }

                    cfg.Host(rabbitMqConnectionString);

                    cfg.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }
}
