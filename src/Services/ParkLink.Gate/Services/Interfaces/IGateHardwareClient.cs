using ParkLink.Gate.Entities;

namespace ParkLink.Gate.Services.Interfaces
{
    public interface IGateHardwareClient
    {
        Task<bool> OpenAsync(GateDevice device,
            CancellationToken cancellationToken = default);
        Task<bool> CloseAsync(GateDevice device,
            CancellationToken cancellationToken = default);
    }
}
