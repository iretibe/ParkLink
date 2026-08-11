using ParkLink.Parking.Dtos.ParkingSlots;
using ParkLink.Shared.Contracts.Enums;
using ParkLink.SharedKernel.Pagination;

namespace ParkLink.Parking.Services
{
    public interface IParkingSlotService
    {
        Task<PagedResult<ParkingSlotDto>> GetParkingSlotsAsync(
            ParkingSlotSearchRequest request,
            CancellationToken cancellationToken = default);
        Task<ParkingSlotDto?> GetParkingSlotByIdAsync(Guid id,
            CancellationToken cancellationToken = default);
        Task<ParkingSlotDto> CreateParkingSlotAsync(
            CreateParkingSlotRequest request,
            CancellationToken cancellationToken = default);
        Task<ParkingSlotDto> UpdateParkingSlotAsync(Guid id,
            UpdateParkingSlotRequest request,
            CancellationToken cancellationToken = default);
        Task UpdateParkingSlotStatusAsync(Guid id,
            ParkingSlotStatus status,
            CancellationToken cancellationToken = default);
        Task DeleteParkingSlotAsync(Guid id,
            CancellationToken cancellationToken = default);
    }
}
