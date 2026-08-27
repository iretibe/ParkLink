using MassTransit;
using ParkLink.SharedKernel.Events.Payment;

namespace ParkLink.Gate.Messaging.Consumers.Payment
{
    public sealed class PaymentRefundedConsumer
        : IConsumer<PaymentRefundedIntegrationEvent>
    {
        private readonly ILogger<PaymentRefundedConsumer> _logger;

        public PaymentRefundedConsumer(ILogger<PaymentRefundedConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<PaymentRefundedIntegrationEvent> context)
        {
            var message = context.Message;

            _logger.LogWarning(
                "Payment refunded. " +
                "PaymentId: {PaymentId}, " +
                "ReservationId: {ReservationId}, " +
                "ReservationNumber: {ReservationNumber}, " +
                "VehicleId: {VehicleId}, " +
                "RefundedAmount: {RefundedAmount} {CurrencyCode}, " +
                "OriginalAmount: {OriginalAmount}, " +
                "PaymentReference: {PaymentReference}, " +
                "ProviderReference: {ProviderReference}",
                message.PaymentId,
                message.ReservationId,
                message.ReservationNumber,
                message.VehicleId,
                message.RefundedAmount,
                message.CurrencyCode,
                message.OriginalAmount,
                message.PaymentReference,
                message.ProviderReference
            );

            // Invalidate associated Gate payment projection/cache.

            return Task.CompletedTask;
        }
    }
}
