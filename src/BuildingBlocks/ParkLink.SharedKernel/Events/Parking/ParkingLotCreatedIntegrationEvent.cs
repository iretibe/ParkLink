namespace ParkLink.SharedKernel.Events.Parking
{
    public sealed record ParkingLotCreatedIntegrationEvent(
        Guid ParkingLotId, string Name, string CountryCode,
        string City, string? Address, double? Latitude, double? Longitude
    ) : IntegrationEvent;
}
