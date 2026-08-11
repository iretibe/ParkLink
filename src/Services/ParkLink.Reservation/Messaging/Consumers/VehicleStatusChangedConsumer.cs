using MassTransit;
using Microsoft.EntityFrameworkCore;
using ParkLink.Reservation.Data;
using ParkLink.Reservation.Enums;
using ParkLink.SharedKernel.Events.Vehicle;

namespace ParkLink.Reservation.Messaging.Consumers
{
    public sealed class VehicleStatusChangedConsumer
        : IConsumer<VehicleStatusChangedIntegrationEvent>
    {
        private readonly ReservationContext _context;
        private readonly ILogger<VehicleStatusChangedConsumer> _logger;

        public VehicleStatusChangedConsumer(ReservationContext context,
            ILogger<VehicleStatusChangedConsumer> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Consume(
            ConsumeContext<VehicleStatusChangedIntegrationEvent> context)
        {
            var message = context.Message;

            using var scope = _logger.BeginScope(
                new Dictionary<string, object?>
                {
                    ["EventId"] = message.EventId,
                    ["VehicleId"] = message.VehicleId,
                    ["OwnerId"] = message.OwnerId,
                    ["CorrelationId"] = context.CorrelationId?.ToString()
                });

            if (!IsUnavailableStatus(message.NewStatus))
            {
                return;
            }

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
                    $"Vehicle status changed to {message.NewStatus}.";
                reservation.CancelledAtUtc = DateTime.UtcNow;
                reservation.UpdatedAtUtc = DateTime.UtcNow;
            }

            if (reservations.Count > 0)
            {
                await _context.SaveChangesAsync(context.CancellationToken);
            }

            _logger.LogInformation(
                "Vehicle {VehicleId} status changed to {Status}. " +
                "Cancelled {Count} future reservations.",
                message.VehicleId,
                message.NewStatus,
                reservations.Count
            );
        }

        private static bool IsUnavailableStatus(string status)
        {
            return 
                status.Equals("Suspended",
                       StringComparison.OrdinalIgnoreCase)
                || status.Equals("Inactive",
                       StringComparison.OrdinalIgnoreCase)
                || status.Equals("Disabled",
                       StringComparison.OrdinalIgnoreCase);
        }
    }
}
