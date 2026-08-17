using ParkLink.Payment.Services;

namespace ParkLink.Payment.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPaymentServices(this IServiceCollection services)
        {
            services.AddScoped<IPaystackWebhookValidator, PaystackWebhookValidator>();
            services.AddScoped<IPaymentService, PaymentService>();

            return services;
        }
    }
}
