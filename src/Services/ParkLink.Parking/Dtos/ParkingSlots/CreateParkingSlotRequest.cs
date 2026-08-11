using ParkLink.Parking.Enums;
using System.ComponentModel.DataAnnotations;

namespace ParkLink.Parking.Dtos.ParkingSlots
{
    public sealed class CreateParkingSlotRequest
    {
        [Required]
        public Guid ParkingZoneId { get; set; }

        [Required]
        [MaxLength(50)]
        public string SlotNumber { get; set; } = string.Empty;

        public ParkingSlotType SlotType { get; set; } = ParkingSlotType.Standard;
    }
}
