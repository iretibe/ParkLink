namespace ParkLink.Gate.Dtos
{
    public sealed record VehicleLookupResult(
        Guid VehicleId, 
        string UserId, 
        string LicensePlate, 
        bool IsActive
    );
}
