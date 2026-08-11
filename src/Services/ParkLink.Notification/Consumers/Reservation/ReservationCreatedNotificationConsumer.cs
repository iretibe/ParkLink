using MassTransit;
using ParkLink.Notification.Enums;
using ParkLink.Notification.Services;
using ParkLink.SharedKernel.Events.Reservation;

namespace ParkLink.Notification.Consumers.Reservation
{
    public sealed class ReservationCreatedNotificationConsumer
        : NotificationConsumerBase<ReservationCreatedIntegrationEvent>
    {
        public ReservationCreatedNotificationConsumer(
            INotificationService notificationService,
            INotificationDispatcher dispatcher,
            ILogger<ReservationCreatedNotificationConsumer> logger)
            : base(notificationService, dispatcher, logger)
        {
        }

        public override async Task Consume(
            ConsumeContext<ReservationCreatedIntegrationEvent> context)
        {
            LogReceived(context);

            var message = context.Message;

            await CreateNotificationAsync(
                context,
                message.UserId,
                nameof(ReservationCreatedIntegrationEvent),
                "Reservation created",
                $"Your reservation {message.ReservationNumber} has been created successfully " +
                    $"for vehicle {message.VehicleId} at parking lot {message.ParkingLotName}. " +
                    $"Your reservation is currently pending confirmation.",
                message.ReservationId,
                "Reservation",
                NotificationType.EmailAndInApp,
                NotificationPriority.Normal,
                $"/reservations/{message.ReservationId}",
                context.CorrelationId?.ToString(),
                cancellationToken: context.CancellationToken
            );
        }
    }
}
