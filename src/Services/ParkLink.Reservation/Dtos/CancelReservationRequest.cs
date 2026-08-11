using System.ComponentModel.DataAnnotations;

namespace ParkLink.Reservation.Dtos
{
    public class CancelReservationRequest
    {
        [Required]
        [MaxLength(500)]
        public string Reason { get; set; } = string.Empty;
    }
}
