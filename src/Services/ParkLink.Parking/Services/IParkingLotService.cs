using ParkLink.Parking.Dtos.ParkingLots;
using ParkLink.SharedKernel.Pagination;

namespace ParkLink.Parking.Services
{
    public interface IParkingLotService
    {
        Task<PagedResult<ParkingLotDetailsDto>> GetParkingLotsAsync(
            ParkingLotSearchRequest request,
            CancellationToken cancellationToken = default);
        Task<ParkingLotDetailsDto?> GetParkingLotByIdAsync(Guid id,
            CancellationToken cancellationToken = default);
        Task<ParkingLotDetailsDto> CreateParkingLotAsync(
            CreateParkingLotRequest request,
            CancellationToken cancellationToken = default);
        Task<ParkingLotDetailsDto> UpdateParkingLotAsync(Guid id,
            UpdateParkingLotRequest request,
            CancellationToken cancellationToken = default);
        Task DeleteParkingLotAsync(Guid id,
            CancellationToken cancellationToken = default);
    }
}
