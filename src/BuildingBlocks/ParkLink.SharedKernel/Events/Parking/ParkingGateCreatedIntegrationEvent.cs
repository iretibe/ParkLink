namespace ParkLink.SharedKernel.Events.Parking
{
    public sealed record ParkingGateCreatedIntegrationEvent(
        Guid ParkingGateId, Guid ParkingLotId, string Name,
        string GateType, string Status, string? DeviceIdentifier,
        string? RfidReaderIdentifier, string? OcrCameraIdentifier
    ) : IntegrationEvent;
}
