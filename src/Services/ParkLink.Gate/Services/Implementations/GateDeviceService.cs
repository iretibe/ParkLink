using Microsoft.EntityFrameworkCore;
using ParkLink.Gate.Data;
using ParkLink.Gate.Dtos;
using ParkLink.Gate.Entities;
using ParkLink.Gate.Enums;
using ParkLink.Gate.Services.Interfaces;

namespace ParkLink.Gate.Services.Implementations
{
    public sealed class GateDeviceService : IGateDeviceService
    {
        private readonly GateContext _context;

        public GateDeviceService(GateContext context)
        {
            _context = context;
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var device = await _context.GateDevices
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (device is null) return false;

            _context.GateDevices.Remove(device);

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<GateDeviceDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.GateDevices
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new GateDeviceDto(
                    x.Id,
                    x.GateId,
                    x.DeviceName,
                    x.DeviceIdentifier,
                    x.Type,
                    x.Status,
                    x.IpAddress,
                    x.Port,
                    x.Manufacturer,
                    x.Model,
                    x.LastSeenAtUtc,
                    x.CreatedAtUtc,
                    x.UpdatedAtUtc))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IReadOnlyCollection<GateDeviceDto>> GetForGateAsync(
            Guid gateId, CancellationToken cancellationToken = default)
        {
            return await _context.GateDevices
                .AsNoTracking()
                .Where(x => x.GateId == gateId)
                .OrderBy(x => x.DeviceName)
                .Select(x => new GateDeviceDto(
                    x.Id,
                    x.GateId,
                    x.DeviceName,
                    x.DeviceIdentifier,
                    x.Type,
                    x.Status,
                    x.IpAddress,
                    x.Port,
                    x.Manufacturer,
                    x.Model,
                    x.LastSeenAtUtc,
                    x.CreatedAtUtc,
                    x.UpdatedAtUtc))
                .ToListAsync(cancellationToken);
        }

        public async Task<GateDeviceDto> RegisterAsync(RegisterGateDeviceRequest request, CancellationToken cancellationToken = default)
        {
            var gateExists = await _context.Gates
                .AnyAsync(x => x.Id == request.GateId, cancellationToken);

            if (!gateExists)
                throw new KeyNotFoundException("Gate was not found.");

            var identifierExists = await _context.GateDevices
                .AnyAsync(
                    x => x.DeviceIdentifier == request.DeviceIdentifier.Trim(),
                    cancellationToken
                );

            if (identifierExists)
                throw new InvalidOperationException(
                    "A device with this identifier already exists.");

            var device = GateDevice.Create(
                request.GateId,
                request.DeviceName,
                request.DeviceIdentifier,
                request.Type,
                request.IpAddress,
                request.Port,
                request.Manufacturer,
                request.Model
            );

            _context.GateDevices.Add(device);

            await _context.SaveChangesAsync(cancellationToken);

            return (await GetByIdAsync(device.Id, cancellationToken))!;
        }

        public async Task<bool> SetStatusAsync(Guid id, DeviceStatus status, CancellationToken cancellationToken = default)
        {
            var device = await _context.GateDevices
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

            if (device is null)
                return false;

            switch (status)
            {
                case DeviceStatus.Online:
                    device.MarkOnline();
                    break;

                case DeviceStatus.Offline:
                    device.MarkOffline();
                    break;

                case DeviceStatus.Faulted:
                    device.MarkFaulted();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(status));
            }

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<GateDeviceDto?> UpdateAsync(Guid id, UpdateGateDeviceRequest request, CancellationToken cancellationToken = default)
        {
            var device = await _context.GateDevices
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

            if (device is null)
                return null;

            device.Update(
                request.DeviceName,
                request.IpAddress,
                request.Port,
                request.Manufacturer,
                request.Model
            );

            await _context.SaveChangesAsync(cancellationToken);

            return await GetByIdAsync(id, cancellationToken);
        }
    }
}
