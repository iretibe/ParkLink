using ParkLink.Parking.Enums;

namespace ParkLink.Parking.Dtos.ParkingGates
{
    public class ParkingGateSearchRequest
    {
        public Guid? ParkingLotId { get; set; }
        public string? Search { get; set; }
        public GateStatus? Status { get; set; }
        public ParkingGateType? GateType { get; set; }
        public int PageNumber { get; set; } = 20;
        public int PageSize { get; set; } = 0;
    }
}
