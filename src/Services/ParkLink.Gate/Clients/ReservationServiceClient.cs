using ParkLink.Gate.Dtos;
using ParkLink.Gate.Interfaces;
using System.Net;

namespace ParkLink.Gate.Clients
{
    public class ReservationServiceClient : IReservationServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ReservationServiceClient> _logger;

        public ReservationServiceClient(HttpClient httpClient,
            ILogger<ReservationServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ReservationAccessResult?> GetActiveReservationAsync(
            Guid vehicleId, Guid gateId, DateTime atUtc, CancellationToken cancellationToken = default)
        {
            var url =
                $"api/reservations/active" +
                $"?vehicleId={vehicleId}" +
                $"&gateId={gateId}" +
                $"&atUtc={Uri.EscapeDataString(atUtc.ToString("O"))}"
            ;

            var response = await _httpClient.GetAsync(url, cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(cancellationToken);

                _logger.LogWarning(
                    "Reservation service returned {StatusCode} for VehicleId {VehicleId}, GateId {GateId}. Response: {Response}",
                    response.StatusCode,
                    vehicleId,
                    gateId,
                    body
                );

                throw new HttpRequestException(
                    $"Reservation service returned {(int)response.StatusCode}.");
            }

            return await response.Content.ReadFromJsonAsync<ReservationAccessResult>(cancellationToken);
        }
    }
}
