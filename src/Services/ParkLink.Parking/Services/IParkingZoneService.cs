using ParkLink.Parking.Dtos.ParkingZones;
using ParkLink.SharedKernel.Pagination;

namespace ParkLink.Parking.Services
{
    public interface IParkingZoneService
    {
        Task<PagedResult<ParkingZoneDto>> GetParkingZonesAsync(
            ParkingZoneSearchRequest request,
            CancellationToken cancellationToken = default);
        Task<ParkingZoneDto?> GetParkingZoneByIdAsync(Guid id,
            CancellationToken cancellationToken = default);
        Task<ParkingZoneDto> CreateParkingZoneAsync(
            CreateParkingZoneRequest request,
            CancellationToken cancellationToken = default);
        Task<ParkingZoneDto> UpdateParkingZoneAsync(Guid id,
            UpdateParkingZoneRequest request,
            CancellationToken cancellationToken = default);
        Task DeleteParkingZoneAsync(Guid id,
            CancellationToken cancellationToken = default);
    }
}
