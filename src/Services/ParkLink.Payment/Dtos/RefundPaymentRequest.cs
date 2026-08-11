namespace ParkLink.Payment.Dtos
{
    public sealed record RefundPaymentRequest(
        decimal? Amount,
        string? Reason
    );
}
