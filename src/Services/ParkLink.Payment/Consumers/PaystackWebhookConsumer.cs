using MassTransit;
using ParkLink.Payment.Messages;
using ParkLink.Payment.Services;

namespace ParkLink.Payment.Consumers
{
    public sealed class PaystackWebhookConsumer : IConsumer<PaystackWebhookReceived>
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaystackWebhookConsumer> _logger;

        public PaystackWebhookConsumer(
            IPaymentService paymentService,
            ILogger<PaystackWebhookConsumer> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PaystackWebhookReceived> context)
        {
            _logger.LogInformation("Processing Paystack webhook.");

            await _paymentService.ProcessPaystackWebhookAsync(
                context.Message.Payload, context.CancellationToken
            );
        }
    }
}
