using MassTransit;
using ParkLink.SharedKernel.Events.Reservation;

namespace ParkLink.Payment.Consumers
{
    public sealed class ReservationCreatedConsumer
        : IConsumer<ReservationCreatedIntegrationEvent>
    {
        private readonly ILogger<ReservationCreatedConsumer> _logger;

        public ReservationCreatedConsumer(ILogger<ReservationCreatedConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<ReservationCreatedIntegrationEvent> context)
        {
            var message = context.Message;

            _logger.LogInformation(
                "Reservation {ReservationId} created. Payment can now be initialized.",
                message.ReservationId
            );

            return Task.CompletedTask;
        }
    }
}
