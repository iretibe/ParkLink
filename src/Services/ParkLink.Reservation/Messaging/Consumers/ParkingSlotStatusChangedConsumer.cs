using MassTransit;
using Microsoft.EntityFrameworkCore;
using ParkLink.Reservation.Data;
using ParkLink.Reservation.Enums;
using ParkLink.Shared.Contracts.Enums;
using ParkLink.SharedKernel.Events.Parking;

namespace ParkLink.Reservation.Messaging.Consumers
{
    public sealed class ParkingSlotStatusChangedConsumer
        : IConsumer<ParkingSlotStatusChangedIntegrationEvent>
    {
        private readonly ReservationContext _context;
        private readonly ILogger<ParkingSlotStatusChangedConsumer> _logger;

        public ParkingSlotStatusChangedConsumer(ReservationContext context, 
            ILogger<ParkingSlotStatusChangedConsumer> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Consume(
            ConsumeContext<ParkingSlotStatusChangedIntegrationEvent> context)
        {
            var message = context.Message;

            using var scope = _logger.BeginScope(
                new Dictionary<string, object?>
                {
                    ["EventId"] = message.EventId,
                    ["ParkingSlotId"] = message.ParkingSlotId,
                    ["ParkingZoneId"] = message.ParkingZoneId,
                    ["ParkingLotId"] = message.ParkingLotId,
                    ["CorrelationId"] = context.CorrelationId?.ToString()
                }
            );

            _logger.LogInformation(
                "Processing parking slot status change. SlotId={ParkingSlotId}, " +
                "PreviousStatus={PreviousStatus}, NewStatus={NewStatus}",
                message.ParkingSlotId,
                message.PreviousStatus,
                message.NewStatus);

            if (!Enum.TryParse<ParkingSlotStatus>(
                message.NewStatus, true, out var newStatus))
            {
                _logger.LogWarning(
                    "Unknown parking slot status {Status} for slot {SlotId}.",
                    message.NewStatus,
                    message.ParkingSlotId);

                return;
            }

            if (newStatus is not (ParkingSlotStatus.Maintenance or ParkingSlotStatus.Disabled))
            {
                return;
            }

            var reservations = await _context.Reservations
                .Where(x =>
                    x.ParkingSlotId == message.ParkingSlotId &&
                    x.Status == ReservationStatus.Confirmed &&
                    x.StartTimeUtc >= DateTime.UtcNow)
                .ToListAsync(context.CancellationToken);

            foreach (var reservation in reservations)
            {
                reservation.Status = ReservationStatus.Cancelled;
                reservation.CancellationReason =
                    $"Parking slot {message.SlotNumber} became {message.NewStatus}.";
                reservation.CancelledAtUtc = DateTime.UtcNow;
                reservation.UpdatedAtUtc = DateTime.UtcNow;

                _logger.LogWarning(
                    "Reservation {ReservationId} cancelled because slot {SlotId} " +
                    "became {Status}.",
                    reservation.Id,
                    message.ParkingSlotId,
                    message.NewStatus
                );
            }

            if (reservations.Count > 0)
            {
                await _context.SaveChangesAsync(context.CancellationToken);
            }
        }
    }
}
