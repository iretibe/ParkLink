using ParkLink.Gate.Dtos;
using ParkLink.Gate.Interfaces;
using System.Net;

namespace ParkLink.Gate.Clients
{
    public class PaymentServiceClient : IPaymentServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PaymentServiceClient> _logger;

        public PaymentServiceClient(HttpClient httpClient,
            ILogger<PaymentServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<PaymentAccessResult?> GetPaymentForReservationAsync(
            Guid reservationId, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync(
                $"api/payments/reservations/{reservationId}",
                cancellationToken)
            ;

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(cancellationToken);

                _logger.LogWarning(
                    "Payment service returned {StatusCode} for ReservationId {ReservationId}. Response: {Response}",
                    response.StatusCode,
                    reservationId,
                    body
                );

                throw new HttpRequestException(
                    $"Payment service returned {(int)response.StatusCode}.");
            }

            return await response
                .Content
                .ReadFromJsonAsync<PaymentAccessResult>(cancellationToken);
        }
    }
}
