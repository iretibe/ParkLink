using ParkLink.Gate.Enums;

namespace ParkLink.Gate.Entities
{
    public sealed class GateAccessAttempt
    {
        private GateAccessAttempt() { }

        public Guid Id { get; private set; }
        public Guid GateId { get; private set; }
        public Guid? VehicleId { get; private set; }
        public string? UserId { get; private set; }
        public Guid? ReservationId { get; private set; }
        public string? LicensePlate { get; private set; }
        public string? RfidTagIdentifier { get; private set; }
        public AccessMethod Method { get; private set; }
        public AccessAttemptStatus Status { get; private set; }
        public AccessDecision Decision { get; private set; }
        public string? DecisionReason { get; private set; }
        public DateTime DetectedAtUtc { get; private set; }
        public DateTime? DecisionAtUtc { get; private set; }
        public DateTime? GateOpenedAtUtc { get; private set; }
        public DateTime? CompletedAtUtc { get; private set; }

        public Gate Gate { get; private set; } = null!;

        public static GateAccessAttempt Create(Guid gateId, 
            AccessMethod method, DateTime detectedAtUtc, 
            string? licensePlate = null, string? rfidTagIdentifier = null)
        {
            return new GateAccessAttempt
            {
                Id = Guid.NewGuid(),
                GateId = gateId,
                Method = method,
                LicensePlate = licensePlate,
                RfidTagIdentifier = rfidTagIdentifier,
                Status = AccessAttemptStatus.Detected,
                Decision = AccessDecision.Pending,
                DetectedAtUtc = detectedAtUtc
            };
        }

        public void SetIdentity(Guid vehicleId, string? userId, Guid? reservationId)
        {
            VehicleId = vehicleId;
            UserId = userId;
            ReservationId = reservationId;
        }

        public void Grant(string reason)
        {
            Status = AccessAttemptStatus.Granted;
            Decision = AccessDecision.Granted;
            DecisionReason = reason;
            DecisionAtUtc = DateTime.UtcNow;
        }

        public void Deny(string reason)
        {
            Status = AccessAttemptStatus.Denied;
            Decision = AccessDecision.Denied;
            DecisionReason = reason;
            DecisionAtUtc = DateTime.UtcNow;
        }

        public void Fail(string reason)
        {
            Status = AccessAttemptStatus.Failed;
            Decision = AccessDecision.Error;
            DecisionReason = reason;
            DecisionAtUtc = DateTime.UtcNow;
        }

        public void MarkGateOpened()
        {
            if (Decision != AccessDecision.Granted)
                throw new InvalidOperationException(
                    "The gate cannot be marked as opened unless access was granted.");

            GateOpenedAtUtc = DateTime.UtcNow;
        }

        public void Complete()
        {
            CompletedAtUtc = DateTime.UtcNow;
        }

        public void StartValidation()
        {
            if (Status != AccessAttemptStatus.Detected)
                throw new InvalidOperationException(
                    "Access attempt is not in a valid state for validation.");

            Status = AccessAttemptStatus.Validating;
        }
    }
}
