using ParkLink.Gate.Enums;

namespace ParkLink.Gate.Dtos
{
    public sealed class CreateGateRequest
    {
        public Guid ParkingLotId { get; init; }
        public string Name { get; init; } = null!;
        public GateType Type { get; init; }
        public string? Description { get; init; }
    }
}
