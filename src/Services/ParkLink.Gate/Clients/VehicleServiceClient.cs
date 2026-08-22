using ParkLink.Gate.Dtos;
using ParkLink.Gate.Interfaces;
using System.Net;

namespace ParkLink.Gate.Clients
{
    public class VehicleServiceClient : IVehicleServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<VehicleServiceClient> _logger;

        public VehicleServiceClient(HttpClient httpClient,
            ILogger<VehicleServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<VehicleLookupResult?> FindByLicensePlateAsync(
            string licensePlate, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(licensePlate);

            var encodedPlate =
                Uri.EscapeDataString(licensePlate.Trim().ToUpperInvariant());

            var response = await _httpClient.GetAsync(
                $"api/vehicles/by-license-plate/{encodedPlate}",
                cancellationToken
            );

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(cancellationToken);

                _logger.LogWarning(
                    "Vehicle service returned {StatusCode} while looking up license plate {LicensePlate}. Response: {Response}",
                    response.StatusCode,
                    licensePlate,
                    body
                );

                throw new HttpRequestException(
                    $"Vehicle service returned {(int)response.StatusCode}.");
            }

            return await response.Content.ReadFromJsonAsync<VehicleLookupResult>(cancellationToken);
        }

        public async Task<VehicleLookupResult?> FindByRfidAsync(
            string rfidTagIdentifier, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(rfidTagIdentifier);

            var encodedTag = Uri.EscapeDataString(rfidTagIdentifier.Trim());

            var response = await _httpClient.GetAsync(
                $"api/vehicles/by-rfid/{encodedTag}", cancellationToken
            );

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(cancellationToken);

                _logger.LogWarning(
                    "Vehicle service returned {StatusCode} while looking up RFID {RfidTag}. Response: {Response}",
                    response.StatusCode,
                    rfidTagIdentifier,
                    body);

                throw new HttpRequestException(
                    $"Vehicle service returned {(int)response.StatusCode}.");
            }

            return await response.Content.ReadFromJsonAsync<VehicleLookupResult>(cancellationToken);
        }
    }
}
