namespace ParkLink.Reservation.Dtos
{
    public sealed class ReservationAvailabilityDto
    {
        public Guid ParkingLotId { get; set; }
        public Guid? ParkingZoneId { get; set; }
        public DateTime StartTimeUtc { get; set; }
        public DateTime EndTimeUtc { get; set; }
        public int TotalSlots { get; set; }
        public int AvailableSlots { get; set; }
        public int ReservedSlots { get; set; }
        public int OccupiedSlots { get; set; }
        public int MaintenanceSlots { get; set; }
        public bool RequestedSlotAvailable { get; set; }
        public Guid? RequestedSlotId { get; set; }
    }
}
