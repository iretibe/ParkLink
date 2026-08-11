using Microsoft.AspNetCore.Mvc;
using ParkLink.Reservation.Dtos;
using ParkLink.Reservation.Services;
using System.Security.Claims;

namespace ParkLink.Reservation.Controllers
{
    public class ReservationHoldsController : BaseController
    {
        private readonly IReservationService _service;
        private readonly ILogger<ReservationHoldsController> _logger;

        public ReservationHoldsController(IReservationService service,
            ILogger<ReservationHoldsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        private string? CurrentUserId =>
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        [HttpPost]
        public async Task<IActionResult> CreateHold(
            [FromBody] CreateReservationHoldRequest request,
            CancellationToken cancellationToken)
        {
            var userId = CurrentUserId;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            try
            {
                var result = await _service.CreateHoldAsync(userId,
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
                _logger.LogWarning(ex, "Unable to create reservation hold.");

                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpDelete("{holdId:guid}")]
        public async Task<IActionResult> ReleaseHold(Guid holdId, CancellationToken cancellationToken)
        {
            var userId = CurrentUserId;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            try
            {
                await _service.ReleaseHoldAsync(holdId, userId, cancellationToken);

                return NoContent();
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
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }
    }
}
