using Duende.AccessTokenManagement;
using Microsoft.Extensions.Options;
using ParkLink.Gate.Clients;
using ParkLink.Gate.Interfaces;

namespace ParkLink.Gate.Extensions
{
    public static class GateHttpClientExtensions
    {
        private static readonly ClientCredentialsClientName ClientName =
            ClientCredentialsClientName.Parse("ParkLinkGateService");

        public static IServiceCollection AddGateHttpClients(
            this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<GateServiceClientOptions>(configuration.GetSection("GateServiceClients"));

            services.AddHttpClient<IVehicleServiceClient, VehicleServiceClient>(
                (serviceProvider, client) =>
                {
                    var options = serviceProvider
                        .GetRequiredService<IOptions<GateServiceClientOptions>>()
                        .Value;

                    client.BaseAddress = new Uri(options.VehicleServiceUrl);
                })
                .AddClientCredentialsTokenHandler(ClientName);

            services.AddHttpClient<IReservationServiceClient, ReservationServiceClient>(
                (serviceProvider, client) =>
                {
                    var options = serviceProvider
                        .GetRequiredService<IOptions<GateServiceClientOptions>>()
                        .Value;

                    client.BaseAddress = new Uri(options.ReservationServiceUrl);
                })
                .AddClientCredentialsTokenHandler(ClientName);

            services.AddHttpClient<IPaymentServiceClient, PaymentServiceClient>(
                (serviceProvider, client) =>
                {
                    var options = serviceProvider
                        .GetRequiredService<IOptions<GateServiceClientOptions>>()
                        .Value;

                    client.BaseAddress = new Uri(options.PaymentServiceUrl);
                })
                .AddClientCredentialsTokenHandler(ClientName);

            return services;
        }
    }
}
