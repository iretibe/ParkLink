namespace ParkLink.Shared.Models.Dashboard
{
    public sealed class DashboardDataModel
    {
        public int TotalSlots { get; set; }
        public int AvailableSlots { get; set; }
        public int OccupiedSlots { get; set; }
        public int ReservedSlots { get; set; }
        public int MaintenanceSlots { get; set; }
        public int TodaysBookings { get; set; }
        public decimal TodaysRevenue { get; set; }
        public string CurrencyCode { get; set; } = "GHS";
        public double AvailablePercentage =>
            TotalSlots == 0
                ? 0
                : AvailableSlots * 100.0 / TotalSlots;
        public double OccupiedPercentage =>
            TotalSlots == 0
                ? 0
                : OccupiedSlots * 100.0 / TotalSlots;
        public double ReservedPercentage =>
            TotalSlots == 0
                ? 0
                : ReservedSlots * 100.0 / TotalSlots;
        public double MaintenancePercentage =>
            TotalSlots == 0
                ? 0
                : MaintenanceSlots * 100.0 / TotalSlots;
        public double OccupancyPercentage =>
            TotalSlots == 0
                ? 0
                : (OccupiedSlots + ReservedSlots) * 100.0 / TotalSlots;
    }
}
