namespace ParkLink.SharedKernel.Events.Vehicle
{
    public sealed record VehicleStatusChangedIntegrationEvent(
        Guid VehicleId, string LicensePlateNumber, string OwnerId, 
        string OldStatus, string NewStatus, string ChangedByUserId, 
        string? Reason
    ) : IntegrationEvent;
}
