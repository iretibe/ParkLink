namespace ParkLink.SharedKernel.Events.Payment
{
    public sealed record PaymentFailedIntegrationEvent(
        Guid PaymentId,
        Guid ReservationId,
        string? PaymentReference,
        string? Reason,
        DateTime FailedAtUtc
    ) : IntegrationEvent;
}
