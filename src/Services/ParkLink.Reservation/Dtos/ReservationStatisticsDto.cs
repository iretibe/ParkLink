namespace ParkLink.Reservation.Dtos
{
    public sealed class ReservationStatisticsDto
    {
        public int TotalReservations { get; set; }
        public int Pending { get; set; }
        public int Held { get; set; }
        public int Confirmed { get; set; }
        public int Active { get; set; }
        public int Completed { get; set; }
        public int Cancelled { get; set; }
        public int Expired { get; set; }
        public int NoShows { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageReservationValue { get; set; }
    }
}
