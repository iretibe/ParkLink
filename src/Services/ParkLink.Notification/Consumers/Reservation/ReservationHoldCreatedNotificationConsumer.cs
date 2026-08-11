using MassTransit;
using ParkLink.Notification.Enums;
using ParkLink.Notification.Services;
using ParkLink.SharedKernel.Events.Reservation;

namespace ParkLink.Notification.Consumers.Reservation
{
    public sealed class ReservationHoldCreatedNotificationConsumer
        : NotificationConsumerBase<ReservationHoldCreatedIntegrationEvent>
    {
        public ReservationHoldCreatedNotificationConsumer(
            INotificationService notificationService,
            INotificationDispatcher dispatcher,
            ILogger<ReservationHoldCreatedNotificationConsumer> logger)
            : base(notificationService, dispatcher, logger)
        {
        }

        public override async Task Consume(
            ConsumeContext<ReservationHoldCreatedIntegrationEvent> context)
        {
            LogReceived(context);

            var message = context.Message;

            await CreateNotificationAsync(
                context,
                message.UserId,
                nameof(ReservationHoldCreatedIntegrationEvent),
                "Parking slot temporarily held",
                $"Your parking slot '{message.ParkingSlotName}' has been temporarily held while your reservation {message.ReservationNumber} is being processed.",
                message.HoldId,
                "ReservationHold",
                NotificationType.InApp | NotificationType.Push,
                NotificationPriority.Normal,
                $"/reservationHolds/{message.HoldId}",
                context.CorrelationId?.ToString(),
                cancellationToken: context.CancellationToken
            );
        }
    }
}
