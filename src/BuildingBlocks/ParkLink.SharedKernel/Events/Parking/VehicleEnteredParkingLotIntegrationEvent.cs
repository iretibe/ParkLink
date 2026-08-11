namespace ParkLink.SharedKernel.Events.Parking
{
    public sealed record VehicleEnteredParkingLotIntegrationEvent(
        Guid ReservationId,
        Guid ParkingLotId,
        Guid ParkingZoneId,
        Guid ParkingSlotId,
        Guid ParkingGateId,
        string UserId,
        Guid VehicleId,
        string? LicensePlateNumber,
        string ReservationNumber,
        string? RfidTag,
        string? OcrPlateNumber,
        DateTime? EnteredAtUtc
    ) : IntegrationEvent;
}
