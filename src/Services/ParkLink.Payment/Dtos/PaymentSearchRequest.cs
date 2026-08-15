using ParkLink.Payment.Enums;

namespace ParkLink.Payment.Dtos
{
    public class PaymentSearchRequest
    {
        public string? Search { get; set; }
        public Guid? ReservationId { get; set; }
        public string? UserId { get; set; }
        public Guid? VehicleId { get; set; }
        public PaymentStatus? Status { get; set; }
        public PaymentMethod? Method { get; set; }
        public string? Provider { get; set; }
        public DateTime? FromDateUtc { get; set; }
        public DateTime? ToDateUtc { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
