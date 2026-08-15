using ParkLink.Reservation.Enums;
using ParkLink.Shared.Contracts.Enums;

namespace ParkLink.Reservation.Dtos
{
    public class ReservationDetailsDto
    {
        public Guid Id { get; set; }
        public string ReservationNumber { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public Guid VehicleId { get; set; }
        public Guid ParkingLotId { get; set; }
        public Guid ParkingZoneId { get; set; }
        public Guid ParkingSlotId { get; set; }
        public string ParkingLotName { get; set; } = string.Empty;
        public string ParkingSlotNumber { get; set; } = string.Empty;
        public ReservationType ReservationType { get; set; }
        public ReservationStatus Status { get; set; }
        public ReservationPaymentStatus PaymentStatus { get; set; }
        public AccessMethod AccessMethod { get; set; }
        public DateTime StartTimeUtc { get; set; }
        public DateTime EndTimeUtc { get; set; }
        public DateTime? ActualEntryTimeUtc { get; set; }
        public DateTime? ActualExitTimeUtc { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;
        public string? PaymentReference { get; set; }
        public string? AccessCredential { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
    }
}
