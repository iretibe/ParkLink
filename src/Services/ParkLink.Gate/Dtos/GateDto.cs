using ParkLink.Gate.Enums;

namespace ParkLink.Gate.Dtos
{
    public sealed record GateDto(
        Guid Id,
        Guid ParkingLotId,
        string Name,
        GateType Type,
        GateStatus Status,
        string? Description,
        DateTime CreatedAtUtc,
        DateTime? UpdatedAtUtc,
        int DeviceCount
    );
}
