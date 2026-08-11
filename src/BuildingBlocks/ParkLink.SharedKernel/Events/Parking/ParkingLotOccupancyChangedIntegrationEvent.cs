namespace ParkLink.SharedKernel.Events.Parking
{
    public sealed record ParkingLotOccupancyChangedIntegrationEvent(
        Guid ParkingLotId, int TotalSlots, int AvailableSlots, 
        int ReservedSlots, int OccupiedSlots, int MaintenanceSlots,
        int DisabledSlots, DateTime CalculatedAtUtc         
    ) : IntegrationEvent;
}
