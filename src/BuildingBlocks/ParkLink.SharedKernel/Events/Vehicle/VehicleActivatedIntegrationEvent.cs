namespace ParkLink.SharedKernel.Events.Vehicle
{
    public sealed record VehicleActivatedIntegrationEvent(
        Guid VehicleId, string OwnerId, string LicensePlateNumber, 
        string ActivatedByUserId) : IntegrationEvent;
}
