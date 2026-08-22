using ParkLink.Gate.Dtos;

namespace ParkLink.Gate.Interfaces
{
    public interface IVehicleServiceClient
    {
        Task<VehicleLookupResult?> FindByRfidAsync(
            string rfidTagIdentifier, 
            CancellationToken cancellationToken = default);
        Task<VehicleLookupResult?> FindByLicensePlateAsync(
            string licensePlate,
            CancellationToken cancellationToken = default);
    }
}
