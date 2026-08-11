using ParkLink.SharedKernel.Pagination;
using ParkLink.Vehicle.Dtos.Vehicles;

namespace ParkLink.Vehicle.Services
{
    public interface IVehicleService
    {
        Task<PagedResult<VehicleListItemDto>> GetVehiclesAsync(
            VehicleSearchRequest request, CancellationToken cancellationToken = default);
        Task<VehicleDetailsDto?> GetVehicleByIdAsync(Guid vehicleId,
            CancellationToken cancellationToken = default);
        Task<VehicleDetailsDto?> GetMyVehicleAsync(Guid vehicleId, string ownerId, 
            CancellationToken cancellationToken = default);
        Task<VehicleDetailsDto> CreateVehicleAsync(string ownerId,
            CreateVehicleRequest request, CancellationToken cancellationToken = default);
        Task<VehicleDetailsDto> UpdateVehicleAsync(Guid vehicleId, string ownerId,
            UpdateVehicleRequest request,
            CancellationToken cancellationToken = default);
        Task DeleteVehicleAsync(Guid vehicleId, string ownerId,
            CancellationToken cancellationToken = default);
        Task VerifyVehicleAsync(Guid vehicleId, string administratorId,
            VehicleStatusRequest? request = null, CancellationToken cancellationToken = default);
        Task SuspendVehicleAsync(Guid vehicleId, string administratorId,
            VehicleStatusRequest? request = null, CancellationToken cancellationToken = default);
        Task ActivateVehicleAsync(Guid vehicleId, string administratorId,
            VehicleStatusRequest? request = null, CancellationToken cancellationToken = default);
    }
}
