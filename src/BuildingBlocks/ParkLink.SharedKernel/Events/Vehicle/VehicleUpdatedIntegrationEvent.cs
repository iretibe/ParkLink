namespace ParkLink.SharedKernel.Events.Vehicle
{
    public sealed record VehicleUpdatedIntegrationEvent(
        Guid VehicleId, string OwnerId, string LicensePlateNumber, 
        string Make, string Model) : IntegrationEvent;
}
