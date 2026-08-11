using MassTransit;
using ParkLink.Vehicle.Data;

namespace ParkLink.Vehicle.Extensions
{
    public static class MessagingExtensions
    {
        public static IServiceCollection AddVehicleMessaging(
            this IServiceCollection services, IConfiguration configuration)
        {
            var rabbitMqConnectionString =
                configuration.GetConnectionString("rabbitmq");

            if (string.IsNullOrWhiteSpace(rabbitMqConnectionString))
            {
                throw new InvalidOperationException(
                    "RabbitMQ connection string 'rabbitmq' was not found.");
            }

            services.AddMassTransit(x =>
            {
                // EF Core Transactional Outbox
                x.AddEntityFrameworkOutbox<VehicleContext>(o =>
                {
                    o.UseSqlServer();

                    // Use the bus outbox so messages published
                    // within application code are stored transactionally.
                    o.UseBusOutbox();
                });

                // RabbitMQ transport
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(rabbitMqConnectionString);

                    cfg.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }
}
