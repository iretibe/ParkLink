using MassTransit;
using Microsoft.EntityFrameworkCore;
using ParkLink.Reservation.Data;
using ParkLink.Reservation.Enums;
using ParkLink.SharedKernel.Events.Vehicle;

namespace ParkLink.Reservation.Messaging.Consumers
{
    public sealed class VehicleDeletedConsumer
        : IConsumer<VehicleDeletedIntegrationEvent>
    {
        private readonly ReservationContext _context;
        private readonly ILogger<VehicleDeletedConsumer> _logger;

        public VehicleDeletedConsumer(ReservationContext context,
            ILogger<VehicleDeletedConsumer> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Consume(
            ConsumeContext<VehicleDeletedIntegrationEvent> context)
        {
            var message = context.Message;

            using var scope = _logger.BeginScope(
                new Dictionary<string, object?>
                {
                    ["EventId"] = message.EventId,
                    ["VehicleId"] = message.VehicleId,
                    ["OwnerId"] = message.OwnerId,
                    ["CorrelationId"] = context.CorrelationId?.ToString()
                }
            );

            var reservations = await _context.Reservations
                .Where(x =>
                    x.VehicleId == message.VehicleId &&
                    (x.Status == ReservationStatus.Pending ||
                     x.Status == ReservationStatus.Held ||
                     x.Status == ReservationStatus.Confirmed))
                .ToListAsync(context.CancellationToken);

            foreach (var reservation in reservations)
            {
                reservation.Status = ReservationStatus.Cancelled;
                reservation.CancellationReason =
                    "The vehicle associated with this reservation was deleted.";
                reservation.CancelledAtUtc = DateTime.UtcNow;
                reservation.UpdatedAtUtc = DateTime.UtcNow;
            }

            if (reservations.Count > 0)
            {
                await _context.SaveChangesAsync(context.CancellationToken);
            }

            _logger.LogInformation(
                "Processed deletion of vehicle {VehicleId}. " +
                "Cancelled {Count} reservations.",
                message.VehicleId,
                reservations.Count
            );
        }
    }
}
