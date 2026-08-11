namespace ParkLink.Reservation.Dtos
{
    public sealed class ReservationDashboardDto
    {
        public int PendingReservations { get; set; }
        public int ActiveReservations { get; set; }
        public int CompletedToday { get; set; }
        public int CancelledToday { get; set; }
        public int NoShowsToday { get; set; }
        public decimal RevenueToday { get; set; }
    }
}
