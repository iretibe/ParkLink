namespace ParkLink.Payment.Enums
{
    public enum PaymentStatus
    {
        Pending = 0,
        Processing = 1,
        Authorized = 2,
        Completed = 3,
        Failed = 4,
        Cancelled = 5,
        RefundPending = 6,
        Refunded = 7,
        PartiallyRefunded = 8
    }
}
