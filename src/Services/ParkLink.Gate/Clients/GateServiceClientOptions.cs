namespace ParkLink.Gate.Clients
{
    public sealed class GateServiceClientOptions
    {
        public string VehicleServiceUrl { get; set; } = null!;
        public string ReservationServiceUrl { get; set; } = null!;
        public string PaymentServiceUrl { get; set; } = null!;
    }
}
