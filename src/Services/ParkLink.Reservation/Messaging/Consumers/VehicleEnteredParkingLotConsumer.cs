using MassTransit;
using Microsoft.EntityFrameworkCore;
using ParkLink.Reservation.Data;
using ParkLink.Reservation.Enums;
using ParkLink.SharedKernel.Events.Parking;

namespace ParkLink.Reservation.Messaging.Consumers
{
    public sealed class VehicleEnteredParkingLotConsumer
        : IConsumer<VehicleEnteredParkingLotIntegrationEvent>
    {
        private readonly ReservationContext _context;
        private readonly ILogger<VehicleEnteredParkingLotConsumer> _logger;

        public VehicleEnteredParkingLotConsumer(ReservationContext context,
            ILogger<VehicleEnteredParkingLotConsumer> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Consume(
            ConsumeContext<VehicleEnteredParkingLotIntegrationEvent> context)
        {
            var message = context.Message;

            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(
                    x =>
                        x.Id == message.ReservationId ||
                        (
                            x.VehicleId == message.VehicleId &&
                            x.ParkingLotId == message.ParkingLotId &&
                            x.Status == ReservationStatus.Confirmed
                        ),
                    context.CancellationToken);

            if (reservation == null)
            {
                _logger.LogWarning(
                    "No matching reservation found for vehicle {VehicleId} entering parking lot {ParkingLotId}.",
                    message.VehicleId,
                    message.ParkingLotId);

                return;
            }

            if (reservation.Status == ReservationStatus.Active)
            {
                return;
            }

            reservation.Status = ReservationStatus.Active;
            reservation.ActualEntryTimeUtc = message.EnteredAtUtc;
            reservation.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync(context.CancellationToken);

            _logger.LogInformation(
                "Reservation {ReservationId} became active after vehicle {VehicleId} entered parking lot.",
                reservation.Id,
                message.VehicleId);
        }
    }
}
