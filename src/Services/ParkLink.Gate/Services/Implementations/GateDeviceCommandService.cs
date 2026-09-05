using Microsoft.EntityFrameworkCore;
using ParkLink.Gate.Data;
using ParkLink.Gate.Dtos;
using ParkLink.Gate.Entities;
using ParkLink.Gate.Enums;
using ParkLink.Gate.Services.Interfaces;

namespace ParkLink.Gate.Services.Implementations
{
    public sealed class GateDeviceCommandService : IGateDeviceCommandService
    {
        private readonly GateContext _context;
        private readonly IGateHardwareClient _hardwareClient;

        public GateDeviceCommandService(GateContext context,
            IGateHardwareClient hardwareClient)
        {
            _context = context;
            _hardwareClient = hardwareClient;
        }

        public async Task<GateDeviceCommandResult> CloseGateAsync(Guid gateId, 
            CancellationToken cancellationToken = default)
        {
            var device = await _context.GateDevices
                .FirstOrDefaultAsync(
                    x => x.GateId == gateId &&
                         x.Type == DeviceType.BarrierGate &&
                         x.Status == DeviceStatus.Online,
                    cancellationToken);

            if (device is null)
            {
                return new GateDeviceCommandResult(false,
                    Guid.Empty, "CLOSE_GATE",
                    "No online barrier gate device was found."
                );
            }

            return await SendCommandAsync(gateId,
                device.Id, "CLOSE_GATE", null, cancellationToken
            );
        }

        public async Task<GateDeviceCommandResult> OpenGateAsync(Guid gateId, 
            Guid accessAttemptId, CancellationToken cancellationToken = default)
        {
            var device = await _context.GateDevices
            .FirstOrDefaultAsync(
                x => x.GateId == gateId &&
                     x.Type == DeviceType.BarrierGate &&
                     x.Status == DeviceStatus.Online,
                cancellationToken);

            if (device is null)
            {
                return new GateDeviceCommandResult(false,
                    Guid.Empty, "OPEN_GATE",
                    "No online barrier gate device was found."
                );
            }

            return await SendCommandAsync(gateId, device.Id,
                "OPEN_GATE", accessAttemptId, cancellationToken
            );
        }

        public async Task<GateDeviceCommandResult> SendCommandAsync(
            Guid gateId, Guid deviceId, string command, 
            Guid? accessAttemptId = null, CancellationToken cancellationToken = default)
        {
            var device = await _context.GateDevices
                .FirstOrDefaultAsync(
                    x => x.Id == deviceId &&
                         x.GateId == gateId,
                    cancellationToken
                );

            if (device?.Status != DeviceStatus.Online)
            {
                return new GateDeviceCommandResult(
                    false,
                    Guid.Empty,
                    command,
                    $"Device is not online. Current status: {device?.Status}.");
            }

            if (device is null)
            {
                return new GateDeviceCommandResult(
                    false, Guid.Empty, command,
                    "Device was not found."
                );
            }

            var commandEntity = GateDeviceCommand.Create(
                gateId, deviceId, command, accessAttemptId
            );

            _context.GateDeviceCommands.Add(commandEntity);

            await _context.SaveChangesAsync(cancellationToken);

            try
            {
                bool success;

                if (command.Equals("OPEN_GATE", StringComparison.OrdinalIgnoreCase))
                {
                    success = await _hardwareClient.OpenAsync(device, cancellationToken);
                }
                else if (command.Equals("CLOSE_GATE", StringComparison.OrdinalIgnoreCase))
                {
                    success = await _hardwareClient.CloseAsync(device, cancellationToken);
                }
                else
                {
                    success = false;
                }

                if (success)
                {
                    commandEntity.Complete();
                }
                else
                {
                    commandEntity.Fail($"Hardware command '{command}' failed.");
                }

                await _context.SaveChangesAsync(cancellationToken);

                return new GateDeviceCommandResult(
                    success,
                    commandEntity.Id,
                    command,
                    success
                        ? null
                        : commandEntity.ErrorMessage,
                    commandEntity.CompletedAtUtc
                );
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                commandEntity.Fail(ex.Message);

                await _context.SaveChangesAsync(cancellationToken);

                return new GateDeviceCommandResult(
                    false,
                    commandEntity.Id,
                    command,
                    ex.Message,
                    commandEntity.CompletedAtUtc
                );
            }
        }
    }
}
