using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkLink.Reservation.Dtos;
using ParkLink.Reservation.Services;

namespace ParkLink.Reservation.Controllers
{
    public class ReservationAccessController : BaseController
    {
        private readonly IReservationService _service;
        private readonly ILogger<ReservationAccessController> _logger;

        public ReservationAccessController(IReservationService service,
            ILogger<ReservationAccessController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost("availability")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CheckAvailability(
            [FromBody] ReservationAvailabilityRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _service.CheckAvailabilityAsync(request, cancellationToken);

            return Ok(result);
        }

        [HttpPost("{reservationId:guid}/check-in")]
        public async Task<IActionResult> CheckIn(Guid reservationId,
            [FromBody] CheckInRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _service.CheckInAsync(reservationId,
                    request, cancellationToken);

                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(
                    ex,
                    "Reservation check-in failed for {ReservationId}.",
                    reservationId);

                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPost("{reservationId:guid}/check-out")]
        public async Task<IActionResult> CheckOut(Guid reservationId,
            [FromBody] CheckOutRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _service.CheckOutAsync(reservationId,
                    request, cancellationToken);

                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(
                    ex,
                    "Reservation check-out failed for {ReservationId}.",
                    reservationId);

                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }
    }
}
