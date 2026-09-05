using MassTransit;
using ParkLink.SharedKernel.Events.Reservation;

namespace ParkLink.Gate.Messaging.Consumers.Reservation
{
    public sealed class ReservationExtendedConsumer
        : IConsumer<ReservationExtendedIntegrationEvent>
    {
        private readonly ILogger<ReservationExtendedConsumer> _logger;

        public ReservationExtendedConsumer(ILogger<ReservationExtendedConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<ReservationExtendedIntegrationEvent> context)
        {
            var message = context.Message;

            _logger.LogInformation(
                "Reservation extended. " +
                "ReservationId: {ReservationId}, " +
                "ReservationNumber: {ReservationNumber}, " +
                "ParkingSlotId: {ParkingSlotId}, " +
                "PreviousEndTimeUtc: {PreviousEndTimeUtc}, " +
                "NewEndTimeUtc: {NewEndTimeUtc}, " +
                "AdditionalAmount: {AdditionalAmount} {CurrencyCode}",
                message.ReservationId,
                message.ReservationNumber,
                message.ParkingSlotId,
                message.PreviousEndTimeUtc,
                message.NewEndTimeUtc,
                message.AdditionalAmount,
                message.CurrencyCode
            );

            // Update the Gate authorization expiry time.

            return Task.CompletedTask;
        }
    }
}
