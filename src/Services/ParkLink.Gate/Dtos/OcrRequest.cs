namespace ParkLink.Gate.Dtos
{
    public sealed record OcrRequest(
        Guid GateId, Guid DeviceId, 
        string ImageReference,
        byte[]? ImageBytes = null
    );
}
