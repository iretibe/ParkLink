namespace ParkLink.SharedKernel.Events.Vehicle
{
    public sealed record VehicleSuspendedIntegrationEvent(
        Guid VehicleId, string OwnerId, string LicensePlateNumber, 
        string SuspendedByUserId, string? Reason = null) : IntegrationEvent;
}
