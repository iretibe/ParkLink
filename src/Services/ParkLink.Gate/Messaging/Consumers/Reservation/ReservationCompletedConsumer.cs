using MassTransit;
using ParkLink.SharedKernel.Events.Reservation;

namespace ParkLink.Gate.Messaging.Consumers.Reservation
{
    public sealed class ReservationCompletedConsumer
        : IConsumer<ReservationCompletedIntegrationEvent>
    {
        private readonly ILogger<ReservationCompletedConsumer> _logger;

        public ReservationCompletedConsumer(ILogger<ReservationCompletedConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<ReservationCompletedIntegrationEvent> context)
        {
            var message = context.Message;

            _logger.LogInformation(
                "Reservation completed. " +
                "ReservationId: {ReservationId}, " +
                "ReservationNumber: {ReservationNumber}, " +
                "VehicleId: {VehicleId}, " +
                "ParkingSlotId: {ParkingSlotId}, " +
                "Amount: {Amount} {CurrencyCode}, " +
                "Entry: {ActualEntryTimeUtc}, " +
                "Exit: {ActualExitTimeUtc}",
                message.ReservationId,
                message.ReservationNumber,
                message.VehicleId,
                message.ParkingSlotId,
                message.Amount,
                message.CurrencyCode,
                message.ActualEntryTimeUtc,
                message.ActualExitTimeUtc
            );

            // Remove/invalidate the active Gate authorization projection.

            return Task.CompletedTask;
        }
    }
}
