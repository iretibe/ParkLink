using ParkLink.Gate.Dtos;
using ParkLink.Gate.Enums;

namespace ParkLink.Gate.Services.Interfaces
{
    public interface IGateDeviceService
    {
        Task<GateDeviceDto?> GetByIdAsync(Guid id,
            CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<GateDeviceDto>> GetForGateAsync(
            Guid gateId, CancellationToken cancellationToken = default);
        Task<GateDeviceDto> RegisterAsync(
            RegisterGateDeviceRequest request,
            CancellationToken cancellationToken = default);
        Task<GateDeviceDto?> UpdateAsync(Guid id,
            UpdateGateDeviceRequest request,
            CancellationToken cancellationToken = default);
        Task<bool> SetStatusAsync(Guid id,
            DeviceStatus status,
            CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id,
            CancellationToken cancellationToken = default);
    }
}
