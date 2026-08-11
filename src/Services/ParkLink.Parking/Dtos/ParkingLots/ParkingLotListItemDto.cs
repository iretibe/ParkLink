using ParkLink.Parking.Enums;

namespace ParkLink.Parking.Dtos.ParkingLots
{
    public class ParkingLotListItemDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string CountryCode { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string? Address { get; set; }

        public ParkingLotStatus Status { get; set; }

        public bool IsActive { get; set; }

        public int ZoneCount { get; set; }

        public int TotalSlotCount { get; set; }

        public int AvailableSlotCount { get; set; }
    }
}
