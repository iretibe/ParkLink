using MassTransit;
using ParkLink.SharedKernel.Events.Payment;

namespace ParkLink.Gate.Messaging.Consumers.Payment
{
    public sealed class PaymentCompletedConsumer
        : IConsumer<PaymentCompletedIntegrationEvent>
    {
        private readonly ILogger<PaymentCompletedConsumer> _logger;

        public PaymentCompletedConsumer(ILogger<PaymentCompletedConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<PaymentCompletedIntegrationEvent> context)
        {
            var message = context.Message;

            _logger.LogInformation(
                "Payment completed. " +
                "PaymentId: {PaymentId}, " +
                "ReservationId: {ReservationId}, " +
                "ReservationNumber: {ReservationNumber}, " +
                "VehicleId: {VehicleId}, " +
                "Amount: {Amount} {CurrencyCode}, " +
                "PaymentReference: {PaymentReference}",
                message.PaymentId,
                message.ReservationId,
                message.ReservationNumber,
                message.VehicleId,
                message.Amount,
                message.CurrencyCode,
                message.PaymentReference
            );

            // Update Gate's local payment projection/cache if required.

            return Task.CompletedTask;
        }
    }
}
