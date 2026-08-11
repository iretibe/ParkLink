namespace ParkLink.SharedKernel.Events.Parking
{
    public sealed record ParkingLotUpdatedIntegrationEvent(
        Guid ParkingLotId, string Name, string CountryCode, 
        string City, string? Address, double? Latitude, 
        double? Longitude, string Status
    ) : IntegrationEvent;
}
