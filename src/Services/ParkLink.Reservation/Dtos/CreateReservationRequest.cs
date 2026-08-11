using ParkLink.Reservation.Enums;
using System.ComponentModel.DataAnnotations;

namespace ParkLink.Reservation.Dtos
{
    public sealed class CreateReservationRequest
    {
        [Required]
        public Guid VehicleId { get; set; }

        [Required]
        public Guid ParkingLotId { get; set; }

        [Required]
        public Guid ParkingZoneId { get; set; }

        [Required]
        public Guid ParkingSlotId { get; set; }

        public ReservationType ReservationType { get; set; }

        public AccessMethod AccessMethod { get; set; }

        [Required]
        public DateTime StartTimeUtc { get; set; }

        [Required]
        public DateTime EndTimeUtc { get; set; }

        [Required]
        [MaxLength(3)]
        public string CurrencyCode { get; set; } = "XOF";
    }
}
