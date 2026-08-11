using ParkLink.Reservation.Enums;

namespace ParkLink.Reservation.Models
{
    public class ReservationHold
    {
        public Guid Id { get; set; }
        public Guid ReservationId { get; set; }
        public Guid ParkingSlotId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ReservationHoldStatus Status { get; set; } = ReservationHoldStatus.Active;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAtUtc { get; set; }
        public DateTime? ConvertedAtUtc { get; set; }
        public DateTime? ReleasedAtUtc { get; set; }
        public DateTime? ExpiredAtUtc { get; set; }
        public Reservation Reservation { get; set; } = default!;
    }
}
