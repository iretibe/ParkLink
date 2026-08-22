using ParkLink.Gate.Enums;

namespace ParkLink.Gate.Entities
{
    public sealed class GateDevice
    {
        private GateDevice() { }

        public Guid Id { get; private set; }
        public Guid GateId { get; private set; }
        public string DeviceName { get; private set; } = null!;
        public string DeviceIdentifier { get; private set; } = null!;
        public DeviceType Type { get; private set; }
        public DeviceStatus Status { get; private set; }
        public string? IpAddress { get; private set; }
        public int? Port { get; private set; }
        public string? Manufacturer { get; private set; }
        public string? Model { get; private set; }
        public DateTime LastSeenAtUtc { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime UpdatedAtUtc { get; private set; }
        public byte[] RowVersion { get; private set; } = [];

        public Gate Gate { get; private set; } = null!;

        public static GateDevice Create(Guid gateId, string deviceName,
            string deviceIdentifier, DeviceType type, string? ipAddress = null,
            int? port = null, string? manufacturer = null, string? model = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(deviceName, nameof(deviceName));

            ArgumentException.ThrowIfNullOrWhiteSpace(deviceIdentifier, nameof(deviceIdentifier));

            return new GateDevice
            {
                Id = Guid.NewGuid(),
                GateId = gateId,
                DeviceName = deviceName.Trim(),
                DeviceIdentifier = deviceIdentifier.Trim(),
                Type = type,
                Status = DeviceStatus.Offline,
                IpAddress = ipAddress,
                Port = port,
                Manufacturer = manufacturer,
                Model = model,
                CreatedAtUtc = DateTime.UtcNow,
            };
        }

        public void Update(string deviceName, string? ipAddress,
            int? port, string? manufacturer, string? model)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(deviceName, nameof(deviceName));

            DeviceName = deviceName.Trim();
            IpAddress = ipAddress;
            Port = port;
            Manufacturer = manufacturer;
            Model = model;
            UpdatedAtUtc = DateTime.UtcNow;
        }

        public void MarkOnline()
        {
            Status = DeviceStatus.Online;
            LastSeenAtUtc = DateTime.UtcNow;
            UpdatedAtUtc = DateTime.UtcNow;
        }

        public void MarkOffline() 
        { 
            Status = DeviceStatus.Offline; 
            UpdatedAtUtc = DateTime.UtcNow;
        }

        public void MarkFaulted()
        {
            Status = DeviceStatus.Faulted;
            UpdatedAtUtc = DateTime.UtcNow;
        }
    }
}
