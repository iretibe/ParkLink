namespace ParkLink.Gate.Entities
{
    public sealed class RfidTag
    {
        private RfidTag() { }

        public Guid Id { get; private set; }
        public string TagIdentifier { get; private set; } = null!;
        public Guid? VehicleId { get; private set; }
        public string? UserId { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime? AssignedAtUtc { get; private set; }
        public DateTime? DeactivatedAtUtc { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }

        public static RfidTag Create(string tagIdentifier)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(tagIdentifier, nameof(tagIdentifier));

            return new RfidTag()
            {
                Id = Guid.NewGuid(),
                TagIdentifier = tagIdentifier.Trim(),
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow
            };
        }

        public void AssignToVehicle(Guid vehicleId)
        {
            VehicleId = vehicleId;
            AssignedAtUtc = DateTime.UtcNow;
        }

        public void AssignToUser(string userId)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(userId, nameof(userId));

            UserId = userId;
        }

        public void Deactivate()
        {
            IsActive = false;
            DeactivatedAtUtc = DateTime.UtcNow;
        }

        public void Activate()
        {
            IsActive = true;
            DeactivatedAtUtc = null;
        }
    }
}
