using System.ComponentModel.DataAnnotations;

namespace ParkLink.Reservation.Dtos
{
    public class ExtendReservationRequest
    {
        [Required]
        public DateTime NewEndTimeUtc { get; set; }

        [MaxLength(500)]
        public string? Reason { get; set; }
    }
}
