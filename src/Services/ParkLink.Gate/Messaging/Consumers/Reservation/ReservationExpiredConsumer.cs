using MassTransit;
using ParkLink.SharedKernel.Events.Reservation;

namespace ParkLink.Gate.Messaging.Consumers.Reservation
{
    public sealed class ReservationExpiredConsumer
        : IConsumer<ReservationExpiredIntegrationEvent>
    {
        private readonly ILogger<ReservationExpiredConsumer> _logger;

        public ReservationExpiredConsumer(ILogger<ReservationExpiredConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<ReservationExpiredIntegrationEvent> context)
        {
            var message = context.Message;

            _logger.LogInformation(
                "Reservation expired. " +
                "ReservationId: {ReservationId}, " +
                "VehicleId: {VehicleId}",
                message.ReservationId,
                message.VehicleId
            );
            
            // Gate reservation projection/cache.

            return Task.CompletedTask;
        }
    }
}
