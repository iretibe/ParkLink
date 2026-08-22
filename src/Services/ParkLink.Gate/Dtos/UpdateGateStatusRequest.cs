using ParkLink.Gate.Enums;

namespace ParkLink.Gate.Dtos
{
    public sealed class UpdateGateStatusRequest
    {
        public GateStatus Status { get; init; }
    }
}
