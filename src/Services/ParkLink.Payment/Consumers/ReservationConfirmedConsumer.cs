using MassTransit;
using ParkLink.SharedKernel.Events.Reservation;

namespace ParkLink.Payment.Consumers
{
    public sealed class ReservationConfirmedConsumer
        : IConsumer<ReservationConfirmedIntegrationEvent>
    {
        private readonly ILogger<ReservationConfirmedConsumer> _logger;

        public ReservationConfirmedConsumer(ILogger<ReservationConfirmedConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<ReservationConfirmedIntegrationEvent> context)
        {
            var message = context.Message;

            _logger.LogInformation(
                "Reservation {ReservationId} confirmed. Payment status should be verified.",
                message.ReservationId);

            return Task.CompletedTask;
        }
    }
}
