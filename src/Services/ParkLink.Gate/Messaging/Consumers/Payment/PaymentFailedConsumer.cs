using MassTransit;
using ParkLink.SharedKernel.Events.Payment;

namespace ParkLink.Gate.Messaging.Consumers.Payment
{
    public sealed class PaymentFailedConsumer
        : IConsumer<PaymentFailedIntegrationEvent>
    {
        private readonly ILogger<PaymentFailedConsumer> _logger;

        public PaymentFailedConsumer(ILogger<PaymentFailedConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<PaymentFailedIntegrationEvent> context)
        {
            var message = context.Message;

            _logger.LogWarning(
                "Payment failed. " +
                "PaymentId: {PaymentId}, " +
                "ReservationId: {ReservationId}, " +
                "ReservationNumber: {ReservationNumber}, " +
                "VehicleId: {VehicleId}, " +
                "Amount: {Amount} {CurrencyCode}, " +
                "FailureReason: {FailureReason}",
                message.PaymentId,
                message.ReservationId,
                message.ReservationNumber,
                message.VehicleId,
                message.Amount,
                message.CurrencyCode,
                message.FailureReason
            );

            // Mark associated payment/reservation projection
            // as unavailable for Gate access if applicable.

            return Task.CompletedTask;
        }
    }
}
