using ParkLink.Gate.Enums;

namespace ParkLink.Gate.Dtos
{
    public sealed record AccessRequest(
        Guid GateId, Guid? DeviceId, AccessMethod Method,
        string? RfidTagIdentifier, string? LicensePlate,
        DateTime DetectedAtUtc
    );
}
