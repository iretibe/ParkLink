namespace ParkLink.Reservation.Dtos
{
    public sealed class ReservationAvailabilityRequest
    {
        public Guid ParkingLotId { get; set; }
        public Guid? ParkingZoneId { get; set; }
        public DateTime StartTimeUtc { get; set; }
        public DateTime EndTimeUtc { get; set; }
    }
}
