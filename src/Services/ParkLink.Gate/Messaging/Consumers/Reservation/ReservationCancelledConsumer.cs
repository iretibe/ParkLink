using MassTransit;
using ParkLink.SharedKernel.Events.Reservation;

namespace ParkLink.Gate.Messaging.Consumers.Reservation
{
    public sealed class ReservationCancelledConsumer
        : IConsumer<ReservationCancelledIntegrationEvent>
    {
        private readonly ILogger<ReservationCancelledConsumer> _logger;

        public ReservationCancelledConsumer(ILogger<ReservationCancelledConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<ReservationCancelledIntegrationEvent> context)
        {
            var message = context.Message;

            _logger.LogInformation(
               "Reservation cancelled. " +
               "ReservationId: {ReservationId}, " +
               "ReservationNumber: {ReservationNumber}, " +
               "VehicleId: {VehicleId}, " +
               "ParkingSlotId: {ParkingSlotId}, " +
               "CancellationReason: {CancellationReason}, " +
               "CancelledByUserId: {CancelledByUserId}, " +
               "CancelledAtUtc: {CancelledAtUtc}",
               message.ReservationId,
               message.ReservationNumber,
               message.VehicleId,
               message.ParkingSlotId,
               message.CancellationReason,
               message.CancelledByUserId,
               message.CancelledAtUtc
           );

            // Gate does not own the reservation.
            // The reservation service remains the source of truth.
            //
            // If Gate maintains a local reservation projection/cache,
            // invalidate/update it here.

            // Invalidate Gate reservation projection/cache.

            return Task.CompletedTask;
        }
    }
}
