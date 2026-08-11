using ParkLink.Reservation.Enums;

namespace ParkLink.Reservation.Dtos
{
    public sealed class ReservationHoldDto
    {
        public Guid Id { get; set; }
        public Guid ParkingSlotId { get; set; }
        public Guid VehicleId { get; set; }
        public ReservationHoldStatus Status { get; set; }
        public DateTime ExpiresAtUtc { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
