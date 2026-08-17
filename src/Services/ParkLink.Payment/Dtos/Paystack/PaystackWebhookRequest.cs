namespace ParkLink.Payment.Dtos.Paystack
{
    public sealed class PaystackWebhookRequest
    {
        public string Event { get; set; } = string.Empty;
        public PaystackWebhookData? Data { get; set; }
    }

    public sealed class PaystackWebhookData
    {
        public long Id { get; set; }
        public string? Status { get; set; }
        public string? Reference { get; set; }
        public long Amount { get; set; }
        public string? Currency { get; set; }
        public string? GatewayResponse { get; set; }
        public DateTime? PaidAt { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? Channel { get; set; }

        public PaystackWebhookCustomer? Customer { get; set; }
    }

    public sealed class PaystackWebhookCustomer
    {
        public long Id { get; set; }
        public string? Email { get; set; }
        public string? CustomerCode { get; set; }
    }
}
