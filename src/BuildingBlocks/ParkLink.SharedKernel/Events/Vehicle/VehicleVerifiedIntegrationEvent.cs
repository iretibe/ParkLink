namespace ParkLink.SharedKernel.Events.Vehicle
{
    public sealed record VehicleVerifiedIntegrationEvent(
        Guid VehicleId, string OwnerId, string LicensePlateNumber, 
        string VerifiedByUserId) : IntegrationEvent;
}
