namespace ParkLink.SharedKernel.Events.Vehicle
{
    public sealed record VehicleDeletedIntegrationEvent(
        Guid VehicleId, string OwnerId, string LicensePlateNumber) 
        : IntegrationEvent;
}
