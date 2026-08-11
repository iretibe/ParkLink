using ParkLink.Reservation.Enums;

namespace ParkLink.Reservation.Dtos
{
    public class ReservationListItemDto
    {
        public Guid Id { get; set; }
        public string ReservationNumber { get; set; } = string.Empty;
        public Guid VehicleId { get; set; }
        public string ParkingLotName { get; set; } = string.Empty;
        public string ParkingSlotNumber { get; set; } = string.Empty;
        public ReservationStatus Status { get; set; }
        public ReservationPaymentStatus PaymentStatus { get; set; }
        public DateTime StartTimeUtc { get; set; }
        public DateTime EndTimeUtc { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;
    }
}
