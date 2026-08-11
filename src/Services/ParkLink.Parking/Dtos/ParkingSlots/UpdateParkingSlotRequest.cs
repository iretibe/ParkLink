using ParkLink.Parking.Enums;
using ParkLink.Shared.Contracts.Enums;
using System.ComponentModel.DataAnnotations;

namespace ParkLink.Parking.Dtos.ParkingSlots
{
    public sealed class UpdateParkingSlotRequest
    {
        [Required]
        [MaxLength(50)]
        public string SlotNumber { get; set; } = string.Empty;

        public ParkingSlotType SlotType { get; set; }

        public ParkingSlotStatus Status { get; set; }

        public bool IsActive { get; set; }
    }
}
