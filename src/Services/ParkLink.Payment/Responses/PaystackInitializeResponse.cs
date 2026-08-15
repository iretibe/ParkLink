namespace ParkLink.Payment.Responses
{
    public class PaystackInitializeResponse
    {
        public bool Status { get; set; }
        public PaystackInitializeData? Data { get; set; }
    }

    public class PaystackInitializeData
    {
        public string? AuthorizationUrl { get; set; }

        public string? Reference { get; set; }
    }
}
