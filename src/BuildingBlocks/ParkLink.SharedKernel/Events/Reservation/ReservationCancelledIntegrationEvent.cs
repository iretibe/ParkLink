namespace ParkLink.SharedKernel.Events.Reservation
{
    public sealed record ReservationCancelledIntegrationEvent(
        Guid ReservationId,
        string ReservationNumber,
        string UserId,
        Guid VehicleId,
        Guid ParkingLotId,
        Guid ParkingZoneId,
        Guid ParkingSlotId,
        string? CancellationReason,
        string? CancelledByUserId,
        DateTime? CancelledAtUtc
    ) : IntegrationEvent;
}
