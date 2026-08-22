namespace ParkLink.Gate.Dtos
{
    public sealed record ReservationAccessResult(
        Guid ReservationId, string ReservationNumber,
        Guid vehicleId, Guid ParkingLotId, DateTime StartAtUtc,
        DateTime EndAtUtc, string Status, bool IsValid
    );
}
