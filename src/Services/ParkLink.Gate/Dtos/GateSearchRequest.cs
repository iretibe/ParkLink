using ParkLink.Gate.Enums;

namespace ParkLink.Gate.Dtos
{
    public sealed class GateSearchRequest
    {
        public Guid? ParkingLotId { get; init; }
        public string? Search { get; init; }
        public GateType? Type { get; init; }
        public GateStatus? Status { get; init; }

        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 20;
    }
}
