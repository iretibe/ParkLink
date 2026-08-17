using MassTransit;
using ParkLink.Payment.Consumers;
using ParkLink.Payment.Data;
using ParkLink.Payment.Providers;

namespace ParkLink.Payment.Extensions
{
    public static class PaymentMessagingExtensions
    {
        public static IServiceCollection AddPaymentMessaging(
            this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumer<ReservationCreatedConsumer>();
                x.AddConsumer<ReservationConfirmedConsumer>();
                x.AddConsumer<PaystackWebhookConsumer>();

                x.AddEntityFrameworkOutbox<PaymentContext>(
                    options =>
                    {
                        options.UseSqlServer();

                        options.QueryDelay = TimeSpan.FromSeconds(1);
                        options.DuplicateDetectionWindow = TimeSpan.FromMinutes(5);
                        options.UseBusOutbox();
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

        public static IServiceCollection AddPaymentProviders(this IServiceCollection services)
        {
            services.AddScoped<IPaymentProviderResolver, PaymentProviderResolver>();
            services.AddScoped<IPaymentProvider, MockPaymentProvider>();

            services.AddHttpClient<PaystackPaymentProvider>(client =>
            {
                client.BaseAddress = new Uri("https://api.paystack.co/");
            });

            services.AddScoped<IPaymentProvider>(provider =>
                provider.GetRequiredService<PaystackPaymentProvider>()
            );

            return services;
        }
    }
}
