namespace ParkLink.Shared.Models.Dashboard
{
    public sealed class ReservationModel
    {
        public Guid Id { get; set; }
        public Guid ParkingId { get; set; }
        public Guid ParkingSlotId { get; set; }
        public Guid UserId { get; set; }
        public string ReservationNumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Initials { get; set; } = string.Empty;
        public string Slot { get; set; } = string.Empty;
        public DateTime StartTimeUtc { get; set; }
        public DateTime EndTimeUtc { get; set; }
        public string Time { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; } = "GHS";
    }
}
