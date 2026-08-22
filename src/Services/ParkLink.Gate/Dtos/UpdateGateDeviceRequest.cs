namespace ParkLink.Gate.Dtos
{
    public sealed class UpdateGateDeviceRequest
    {
        public string DeviceName { get; init; } = null!;
        public string? IpAddress { get; init; }
        public int? Port { get; init; }
        public string? Manufacturer { get; init; }
        public string? Model { get; init; }
    }
}
