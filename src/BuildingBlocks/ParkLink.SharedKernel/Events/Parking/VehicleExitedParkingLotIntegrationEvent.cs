namespace ParkLink.SharedKernel.Events.Parking
{
    public sealed record VehicleExitedParkingLotIntegrationEvent(
        Guid ReservationId,
        Guid ParkingLotId,
        Guid ParkingZoneId,
        Guid ParkingSlotId,
        Guid ParkingGateId,
        Guid VehicleId,
        string UserId,
        string? LicensePlateNumber,
        string ReservationNumber,
        string? RfidTag,
        string? OcrPlateNumber,
        DateTime? ExitedAtUtc
    ) : IntegrationEvent;
}
