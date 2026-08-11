using MassTransit;
using ParkLink.Notification.Enums;
using ParkLink.Notification.Services;
using ParkLink.SharedKernel.Events.Parking;

namespace ParkLink.Notification.Consumers.Reservation
{
    public sealed class ReservationCheckOutNotificationConsumer
        : NotificationConsumerBase<VehicleExitedParkingLotIntegrationEvent>
    {
        public ReservationCheckOutNotificationConsumer(
            INotificationService notificationService,
            INotificationDispatcher dispatcher,
            ILogger<ReservationCheckOutNotificationConsumer> logger)
            : base(notificationService, dispatcher, logger)
        {
        }

        public override async Task Consume(
            ConsumeContext<VehicleExitedParkingLotIntegrationEvent> context)
        {
            LogReceived(context);

            var message = context.Message;

            await CreateNotificationAsync(
                context,
                message.UserId,
                nameof(VehicleExitedParkingLotIntegrationEvent),
                "Vehicle checked out",
                $"Vehicle {message.LicensePlateNumber} has checked out from reservation {message.ReservationNumber}.",
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
