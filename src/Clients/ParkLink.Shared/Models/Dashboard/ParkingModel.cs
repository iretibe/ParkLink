namespace ParkLink.Shared.Models.Dashboard
{
    public sealed class ParkingModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public int TotalSlots { get; set; }
        public int AvailableSlots { get; set; }
        public int OccupiedSlots { get; set; }
        public int ReservedSlots { get; set; }
        public int MaintenanceSlots { get; set; }
        public bool IsActive { get; set; } = true;
        public double OccupancyPercentage =>
            TotalSlots == 0
                ? 0
                : (OccupiedSlots + ReservedSlots) * 100.0 / TotalSlots;
    }
}
