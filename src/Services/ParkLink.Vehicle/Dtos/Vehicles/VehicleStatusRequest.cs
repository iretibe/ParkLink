using System.ComponentModel.DataAnnotations;

namespace ParkLink.Vehicle.Dtos.Vehicles
{
    public class VehicleStatusRequest
    {
        [MaxLength(500)]
        public string? Reason { get; set; }
    }
}
