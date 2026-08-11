namespace ParkLink.SharedKernel.Events.Parking
{
    public sealed record GateAccessRequestedIntegrationEvent(
        Guid RequestId, Guid ParkingLotId, Guid ParkingGateId, 
        Guid VehicleId, string? LicensePlateNumber, string? RfidTagId,
        string? OcrPlateNumber, string AccessDirection, DateTime RequestedAtUtc
    ) : IntegrationEvent;
}
