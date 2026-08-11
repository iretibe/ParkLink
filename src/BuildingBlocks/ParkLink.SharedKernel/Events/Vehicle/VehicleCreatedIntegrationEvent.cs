namespace ParkLink.SharedKernel.Events.Vehicle
{
    public sealed record VehicleCreatedIntegrationEvent(
        Guid VehicleId, string OwnerId, 
        string LicensePlateNumber, string Make, string Model, 
        string VehicleType) : IntegrationEvent;
}
