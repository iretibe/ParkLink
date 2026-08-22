using ParkLink.Gate.Dtos;

namespace ParkLink.Gate.Services.Interfaces
{
    public interface IGateDeviceCommandService
    {
        Task<GateDeviceCommandResult> OpenGateAsync(
            Guid gateId, Guid accessAttemptId,
            CancellationToken cancellationToken = default);
        Task<GateDeviceCommandResult> CloseGateAsync(
            Guid gateId, CancellationToken cancellationToken = default);
        Task<GateDeviceCommandResult> SendCommandAsync(
            Guid gateId, Guid deviceId, string command,
            Guid? accessAttemptId = null,
            CancellationToken cancellationToken = default);
    }
}
