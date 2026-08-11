namespace ParkLink.Reservation.Dtos
{
    public sealed class ReservationAccessRequest
    {
        public Guid? ReservationId { get; set; }
        public string? RfidTag { get; set; }
        public string? LicensePlateNumber { get; set; }
        public string? OcrDetectedPlateNumber { get; set; }
        public string? QrCode { get; set; }
        public string GateIdentifier { get; set; } = string.Empty;
        public DateTime DetectedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
