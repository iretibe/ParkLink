namespace ParkLink.Payment.Enums
{
    public enum PaymentTransactionType
    {
        Authorization = 0,
        Capture = 1,
        Payment = 2,
        Refund = 3,
        PartialRefund = 4
    }
}
