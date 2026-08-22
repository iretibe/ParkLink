using ParkLink.Gate.Enums;

namespace ParkLink.Gate.Dtos
{
    public sealed class RegisterGateDeviceRequest
    {
        public Guid GateId { get; init; }
        public string DeviceName { get; init; } = null!;
        public string DeviceIdentifier { get; init; } = null!;
        public DeviceType Type { get; init; }
        public string? IpAddress { get; init; }
        public int? Port { get; init; }
        public string? Manufacturer { get; init; }
        public string? Model { get; init; }
    }
}
