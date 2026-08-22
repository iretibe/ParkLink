namespace ParkLink.Gate.Entities
{
    public sealed class GateDeviceCommand
    {
        private GateDeviceCommand() { }

        public Guid Id { get; private set; }
        public Guid GateId { get; private set; }
        public Guid DeviceId { get; private set; }
        public Guid? AccessAttemptId { get; private set; }
        public string Command { get; private set; } = null!;
        public bool Successful { get; private set; }
        public string? ErrorMessage { get; private set; }
        public DateTime RequestedAtUtc { get; private set; }
        public DateTime? CompletedAtUtc { get; private set; }

        public static GateDeviceCommand Create(Guid gateId,
            Guid deviceId, string command, Guid? accessAttemptId = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(command, nameof(command));

            return new GateDeviceCommand
            {
                Id = Guid.NewGuid(),
                GateId = gateId,
                DeviceId = deviceId,
                AccessAttemptId = accessAttemptId,
                Command = command.Trim(),
                RequestedAtUtc = DateTime.UtcNow,
            };
        }

        public void Complete()
        {
            Successful = true;
            CompletedAtUtc = DateTime.UtcNow;
        }

        public void Fail(string errorMessage)
        {
            Successful = false;
            ErrorMessage = errorMessage;
            CompletedAtUtc = DateTime.UtcNow;
        }
    }
}
