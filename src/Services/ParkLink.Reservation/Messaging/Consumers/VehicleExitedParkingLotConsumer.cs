using MassTransit;
using Microsoft.EntityFrameworkCore;
using ParkLink.Reservation.Data;
using ParkLink.Reservation.Enums;
using ParkLink.SharedKernel.Events.Parking;

namespace ParkLink.Reservation.Messaging.Consumers
{
    public sealed class VehicleExitedParkingLotConsumer
        : IConsumer<VehicleExitedParkingLotIntegrationEvent>
    {
        private readonly ReservationContext _context;
        private readonly ILogger<VehicleExitedParkingLotConsumer> _logger;

        public VehicleExitedParkingLotConsumer(ReservationContext context,
            ILogger<VehicleExitedParkingLotConsumer> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Consume(
            ConsumeContext<VehicleExitedParkingLotIntegrationEvent> context)
        {
            var message = context.Message;

            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(
                    x =>
                        x.Id == message.ReservationId ||
                        (
                            x.VehicleId == message.VehicleId &&
                            x.ParkingLotId == message.ParkingLotId &&
                            x.Status == ReservationStatus.Active
                        ),
                    context.CancellationToken);

            if (reservation == null)
            {
                _logger.LogWarning(
                    "No active reservation found for vehicle {VehicleId} exiting parking lot {ParkingLotId}.",
                    message.VehicleId,
                    message.ParkingLotId);

                return;
            }

            reservation.Status = ReservationStatus.Completed;
            reservation.ActualExitTimeUtc = message.ExitedAtUtc;
            reservation.CompletedAtUtc = message.ExitedAtUtc;
            reservation.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync(context.CancellationToken);

            _logger.LogInformation(
                "Reservation {ReservationId} completed after vehicle {VehicleId} exited parking lot.",
                reservation.Id,
                message.VehicleId);
        }
    }
}
