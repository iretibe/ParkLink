using ParkLink.Reservation.Enums;

namespace ParkLink.Reservation.Dtos
{
    public class ReservationSearchRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? Search { get; set; }
        public string? UserId { get; set; }
        public Guid? ParkingLotId { get; set; }
        public Guid? ParkingZoneId { get; set; }
        public Guid? ParkingSlotId { get; set; }
        public Guid? VehicleId { get; set; }
        public ReservationStatus? Status { get; set; }
        public ReservationPaymentStatus? PaymentStatus { get; set; }
        public ReservationType? ReservationType { get; set; }
        public DateTime? StartDateUtc { get; set; }
        public DateTime? EndDateUtc { get; set; }
    }
}
