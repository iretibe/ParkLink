namespace ParkLink.Shared.Contracts.Enums
{
    public enum ReservationPaymentStatus
    {
        Pending = 0,
        Authorized = 1,
        Paid = 2,
        Failed = 3,
        Refunded = 4,
        PartiallyRefunded = 5
    }
}
