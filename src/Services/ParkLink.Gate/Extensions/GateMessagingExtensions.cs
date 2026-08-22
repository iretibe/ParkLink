using MassTransit;
using ParkLink.Gate.Data;

namespace ParkLink.Gate.Extensions
{
    public static class GateMessagingExtensions
    {
        public static IServiceCollection AddGateMessaging(
            this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMassTransit(x =>
            {
                x.AddEntityFrameworkOutbox<GateContext>(o =>
                {
                    o.UseSqlServer();
                    o.QueryDelay = TimeSpan.FromSeconds(1);
                    o.DuplicateDetectionWindow = TimeSpan.FromMinutes(5);
                    o.UseBusOutbox();
                });

                x.UsingRabbitMq((context, cfg) =>
                {
                    var host = configuration["RabbitMQ:Host"] ?? "localhost";
                    var username = configuration["RabbitMQ:Username"] ?? "guest";
                    var password = configuration["RabbitMQ:Password"] ?? "guest";

                    cfg.Host(host, "/", h =>
                    {
                        h.Username(username);
                        h.Password(password);
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }
}
