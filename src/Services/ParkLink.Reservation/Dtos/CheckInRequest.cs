namespace ParkLink.Reservation.Dtos
{
    public sealed class CheckInRequest
    {
        public Guid ParkingGateId { get; set; }
        public string? RfidTag { get; set; }
        public string? OcrPlateNumber { get; set; }
        public string? LicensePlateNumber { get; set; }
    }
}
