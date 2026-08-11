using MassTransit;
using ParkLink.Notification.Enums;
using ParkLink.Notification.Services;
using ParkLink.SharedKernel.Events.Parking;

namespace ParkLink.Notification.Consumers.Reservation
{
    public sealed class ReservationCheckedInNotificationConsumer
        : NotificationConsumerBase<VehicleEnteredParkingLotIntegrationEvent>
    {
        public ReservationCheckedInNotificationConsumer(
            INotificationService notificationService,
            INotificationDispatcher dispatcher,
            ILogger<ReservationCheckedInNotificationConsumer> logger)
            : base(notificationService, dispatcher, logger)
        {
        }

        public override async Task Consume(
            ConsumeContext<VehicleEnteredParkingLotIntegrationEvent> context)
        {
            LogReceived(context);

            var message = context.Message;

            await CreateNotificationAsync(
                context,
                message.UserId,
                nameof(VehicleEnteredParkingLotIntegrationEvent),
                "Vehicle checked in",
                $"Vehicle {message.LicensePlateNumber} has checked in for reservation {message.ReservationNumber}.",
                message.ReservationId,
                "Reservation",
                NotificationType.InApp | NotificationType.Push,
                NotificationPriority.Normal,
                $"/reservations/{message.ReservationId}",
                context.CorrelationId?.ToString(),
                cancellationToken: context.CancellationToken
            );
        }
    }
}
