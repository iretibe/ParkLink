namespace ParkLink.Payment.Dtos
{
    public sealed class PaymentStatisticsDto
    {
        public int TotalPayments { get; init; }
        public int PendingPayments { get; init; }
        public int ProcessingPayments { get; init; }
        public int AuthorizedPayments { get; init; }
        public int CompletedPayments { get; init; }
        public int FailedPayments { get; init; }
        public int RefundedPayments { get; init; }
        public int PartiallyRefundedPayments { get; init; }
        public decimal TotalAmount { get; init; }
        public decimal CompletedAmount { get; init; }
        public decimal RefundedAmount { get; init; }
        public DateTime? LastPaymentAtUtc { get; init; }
    }
}
