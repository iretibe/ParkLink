using ParkLink.Gate.Enums;

namespace ParkLink.Gate.Dtos
{
    public sealed record GateDeviceDto(
        Guid Id,
        Guid GateId,
        string DeviceName,
        string DeviceIdentifier,
        DeviceType Type,
        DeviceStatus Status,
        string? IpAddress,
        int? Port,
        string? Manufacturer,
        string? Model,
        DateTime LastSeenAtUtc,
        DateTime CreatedAtUtc,
        DateTime UpdatedAtUtc
    );
}
