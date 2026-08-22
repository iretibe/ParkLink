using ParkLink.Gate.Entities;
using ParkLink.Gate.Services.Interfaces;

namespace ParkLink.Gate.Hardware
{
    public class SimulatedGateHardwareClient : IGateHardwareClient
    {
        private readonly ILogger<SimulatedGateHardwareClient> _logger;

        public SimulatedGateHardwareClient(ILogger<SimulatedGateHardwareClient> logger)
        {
            _logger = logger;
        }

        public Task<bool> CloseAsync(GateDevice device, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(
                "SIMULATED CLOSE GATE: {DeviceName} ({DeviceIdentifier})",
                device.DeviceName,
                device.DeviceIdentifier
            );

            return Task.FromResult(true);
        }

        public Task<bool> OpenAsync(GateDevice device, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(
                "SIMULATED OPEN GATE: {DeviceName} ({DeviceIdentifier})",
                device.DeviceName,
                device.DeviceIdentifier
            );

            return Task.FromResult(true);
        }
    }
}
