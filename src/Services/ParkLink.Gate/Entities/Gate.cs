using ParkLink.Gate.Enums;

namespace ParkLink.Gate.Entities
{
    public sealed class Gate
    {
        private Gate() { }

        public Guid Id { get; private set; }
        public Guid ParkingLotId { get; private set; }
        public string Name { get; private set; } = null!;
        public GateType Type { get; private set; }
        public GateStatus Status { get; private set; }
        public string? Description { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime? UpdatedAtUtc { get; private set; }
        public byte[] RowVersion { get; private set; } = [];

        public ICollection<GateDevice> Devices { get; private set; } = new List<GateDevice>();
        public ICollection<GateAccessAttempt> AccessAttempts { get; private set; } = new List<GateAccessAttempt>();

        public static Gate Create(Guid parkingLotId, string name, 
            GateType type, string? description = null)
        {
            return new Gate
            {
                Id = Guid.NewGuid(),
                ParkingLotId = parkingLotId,
                Name = name.Trim(),
                Type = type,
                Status = GateStatus.Offline,
                Description = description,
                CreatedAtUtc = DateTime.UtcNow
            };
        }

        public void SetStatus(GateStatus status)
        {
            Status = status;
            UpdatedAtUtc = DateTime.UtcNow;
        }

        public void Update(string name, string? description)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));

            Name = name.Trim();
            Description = description;
            UpdatedAtUtc = DateTime.UtcNow;
        }
    }
}
