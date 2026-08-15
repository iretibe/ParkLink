namespace ParkLink.Payment.Responses
{
    public class PaystackVerifyResponse
    {
        public bool Status { get; set; }
        public PaystackVerifyData? Data { get; set; }
    }

    public class PaystackVerifyData
    {
        public string? Status { get; set; }
        public string? Reference { get; set; }
    }
}
