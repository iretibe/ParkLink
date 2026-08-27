using Duende.AccessTokenManagement;
using ParkLink.Gate.Clients;
using ParkLink.Gate.Interfaces;

namespace ParkLink.Gate.Extensions
{
    public static class GateHttpClientExtensions
    {
        private static readonly ClientCredentialsClientName ClientName =
            ClientCredentialsClientName.Parse("ParkLinkGateService");

        public static IServiceCollection AddGateHttpClients(this IServiceCollection services)
        {
            services
                .AddHttpClient<IVehicleServiceClient, VehicleServiceClient>(
                    client =>
                    {
                        client.BaseAddress = new Uri("https://parking-vehicle");
                    })
                .AddClientCredentialsTokenHandler(ClientName);

            services
                .AddHttpClient<IReservationServiceClient, ReservationServiceClient>(
                    client =>
                    {
                        client.BaseAddress = new Uri("https://parking-reservation");
                    })
                .AddClientCredentialsTokenHandler(ClientName);

            services
                .AddHttpClient<IPaymentServiceClient, PaymentServiceClient>(
                    client =>
                    {
                        client.BaseAddress = new Uri("https://parking-payment");
                    })
                .AddClientCredentialsTokenHandler(ClientName);

            return services;
        }
    }
}
