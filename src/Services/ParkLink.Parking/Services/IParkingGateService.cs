using ParkLink.Parking.Dtos.ParkingGates;
using ParkLink.Parking.Enums;
using ParkLink.SharedKernel.Pagination;

namespace ParkLink.Parking.Services
{
    public interface IParkingGateService
    {
        Task<PagedResult<ParkingGateDto>> GetParkingGatesAsync(
            ParkingGateSearchRequest request,
            CancellationToken cancellationToken = default);
        Task<ParkingGateDto?> GetParkingGateByIdAsync(Guid id,
            CancellationToken cancellationToken = default);
        Task<ParkingGateDto> CreateParkingGateAsync(
            CreateParkingGateRequest request,
            CancellationToken cancellationToken = default);
        Task<ParkingGateDto> UpdateParkingGateAsync(Guid id,
            UpdateParkingGateRequest request,
            CancellationToken cancellationToken = default);
        Task UpdateParkingGateStatusAsync(Guid gateId, 
            GateStatus status, CancellationToken cancellationToken = default);
        Task DeleteParkingGateAsync(Guid id,
            CancellationToken cancellationToken = default);
    }
}
