using System.ComponentModel.DataAnnotations;

namespace ParkLink.Reservation.Dtos
{
    public sealed class CreateReservationHoldRequest
    {
        [Required]
        public Guid VehicleId { get; set; }

        [Required]
        public Guid ParkingLotId { get; set; }

        [Required]
        public Guid ParkingZoneId { get; set; }

        [Required]
        public Guid ParkingSlotId { get; set; }

        public int HoldMinutes { get; set; } = 15;
    }
}
