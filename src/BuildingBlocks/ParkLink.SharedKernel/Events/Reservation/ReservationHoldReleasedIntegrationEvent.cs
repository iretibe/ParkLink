namespace ParkLink.SharedKernel.Events.Reservation
{
    public sealed record ReservationHoldReleasedIntegrationEvent(
        Guid HoldId,
        string UserId,
        Guid ReservationId,
        //Guid ParkingLotId,
        Guid ParkingSlotId,
        string ParkingSlotName,
        DateTime ReleasedAtUtc,
        string Reason
    ) : IntegrationEvent;
}
