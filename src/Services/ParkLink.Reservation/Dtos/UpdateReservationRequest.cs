using ParkLink.Reservation.Enums;

namespace ParkLink.Reservation.Dtos
{
    public sealed class UpdateReservationRequest
    {
        public DateTime StartTimeUtc { get; set; }
        public DateTime EndTimeUtc { get; set; }
        public ReservationType ReservationType { get; set; }
        public AccessMethod AccessMethod { get; set; }
    }
}
