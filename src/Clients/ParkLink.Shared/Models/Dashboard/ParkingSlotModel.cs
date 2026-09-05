namespace ParkLink.Shared.Models.Dashboard
{
    public sealed class ParkingSlotModel
    {
        public Guid Id { get; set; }
        public Guid ParkingId { get; set; }
        public string Row { get; set; } = string.Empty;
        public int Number { get; set; }
        public string Status { get; set; } = "Available";
        public string? Label { get; set; }
        public bool IsAccessible { get; set; }
        public bool HasChargingStation { get; set; }
    }
}
