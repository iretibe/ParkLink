using ParkLink.Gate.Dtos;
using ParkLink.Gate.Enums;
using ParkLink.SharedKernel.Pagination;

namespace ParkLink.Gate.Services.Interfaces
{
    public interface IGateService
    {
        Task<GateDto?> GetByIdAsync(Guid id,
            CancellationToken cancellationToken = default);
        Task<PaginatedResult<GateDto>> SearchAsync(
            GateSearchRequest request,
            CancellationToken cancellationToken = default);
        Task<GateDto> CreateAsync(Guid parkingLotId,
            string name, GateType type, string? description,
            CancellationToken cancellationToken = default);
        Task<GateDto?> UpdateAsync(Guid id,
            UpdateGateRequest request,
            CancellationToken cancellationToken = default);
        Task<bool> UpdateStatusAsync(Guid id,
            GateStatus status,
            CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default);
    }
}
