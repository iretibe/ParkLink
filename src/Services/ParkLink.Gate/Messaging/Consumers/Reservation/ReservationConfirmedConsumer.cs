using MassTransit;
using ParkLink.SharedKernel.Events.Reservation;

namespace ParkLink.Gate.Messaging.Consumers.Reservation
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
                "Reservation confirmed. " +
                "ReservationId: {ReservationId}, " +
                "ReservationNumber: {ReservationNumber}, " +
                "VehicleId: {VehicleId}, " +
                "ParkingSlotId: {ParkingSlotId}, " +
                "AccessMethod: {AccessMethod}, " +
                "PaymentReference: {PaymentReference}",
                message.ReservationId,
                message.ReservationNumber,
                message.VehicleId,
                message.ParkingSlotId,
                message.AccessMethod,
                message.PaymentReference
            );

            // Update Gate's local access authorization projection.
            //
            // The Gate service can use this projection to determine whether
            // a vehicle should be allowed through a gate without making a
            // synchronous Reservation API call for every vehicle detection.

            return Task.CompletedTask;
        }
    }
}
