using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkLink.Reservation.Dtos;
using ParkLink.Reservation.Services;
using System.Security.Claims;

namespace ParkLink.Reservation.Controllers
{
    public class ReservationsController : BaseController
    {
        private readonly IReservationService _service;
        private readonly ILogger<ReservationsController> _logger;

        public ReservationsController(IReservationService service,
            ILogger<ReservationsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        private string? CurrentUserId =>
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        [HttpGet]
        [Authorize(Policy = "ReservationManagement")]
        public async Task<IActionResult> GetReservations(
            [FromQuery] ReservationSearchRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _service.GetReservationsAsync(request, cancellationToken);

            return Ok(result);
        }

        [HttpGet("mine")]
        public async Task<IActionResult> GetMyReservations(
            [FromQuery] ReservationSearchRequest request,
            CancellationToken cancellationToken)
        {
            var userId = CurrentUserId;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            var result = await _service.GetMyReservationsAsync(userId,
                request, cancellationToken);

            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetReservation(Guid id,
            CancellationToken cancellationToken)
        {
            var result = await _service.GetReservationByIdAsync(id, cancellationToken);

            if (result is null)
            {
                return NotFound(new
                {
                    message = $"Reservation '{id}' was not found."
                });
            }

            return Ok(result);
        }

        [HttpGet("mine/{id:guid}")]
        public async Task<IActionResult> GetMyReservation(Guid id, CancellationToken cancellationToken)
        {
            var userId = CurrentUserId;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            var result = await _service.GetMyReservationAsync(id, userId, cancellationToken);

            if (result is null)
            {
                return NotFound(new
                {
                    message = $"Reservation '{id}' was not found."
                });
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateReservation(
            [FromBody] CreateReservationRequest request,
            CancellationToken cancellationToken)
        {
            var userId = CurrentUserId;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            try
            {
                var result = await _service.CreateReservationAsync(userId, 
                    request, cancellationToken);

                return CreatedAtAction(
                    nameof(GetReservation),
                    new { id = result.Id },
                    result);
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

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateReservation(Guid id,
            [FromBody] UpdateReservationRequest request,
            CancellationToken cancellationToken)
        {
            var userId = CurrentUserId;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            try
            {
                var result = await _service.UpdateReservationAsync(id,
                    userId, request, cancellationToken);

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
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPatch("{id:guid}/extend")]
        public async Task<IActionResult> ExtendReservation(Guid id,
            [FromBody] ExtendReservationRequest request,
            CancellationToken cancellationToken)
        {
            var userId = CurrentUserId;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            try
            {
                var result = await _service.ExtendReservationAsync(id,
                    userId, request, cancellationToken);

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
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPost("{id:guid}/cancel")]
        public async Task<IActionResult> CancelReservation(Guid id,
            [FromBody] CancelReservationRequest request,
            CancellationToken cancellationToken)
        {
            var userId = CurrentUserId;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            try
            {
                await _service.CancelReservationAsync(id, userId, request, cancellationToken);

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

        [HttpPost("{id:guid}/confirm")]
        [Authorize(Policy = "ReservationManagement")]
        public async Task<IActionResult> ConfirmReservation(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _service.ConfirmReservationAsync(id, cancellationToken);

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
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPost("{id:guid}/activate")]
        [Authorize(Policy = "ReservationManagement")]
        public async Task<IActionResult> ActivateReservation(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _service.ActivateReservationAsync(id, cancellationToken);

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
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPost("{id:guid}/complete")]
        [Authorize(Policy = "ReservationManagement")]
        public async Task<IActionResult> CompleteReservation(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _service.CompleteReservationAsync(id, cancellationToken);

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
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPost("{id:guid}/expire")]
        [Authorize(Policy = "ReservationManagement")]
        public async Task<IActionResult> ExpireReservation(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                await _service.ExpireReservationAsync(id, cancellationToken);

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

        [HttpPost("{id:guid}/no-show")]
        [Authorize(Policy = "ReservationManagement")]
        public async Task<IActionResult> MarkNoShow(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                await _service.MarkReservationAsNoShowAsync(id, cancellationToken);

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

        [HttpGet("statistics")]
        [Authorize(Policy = "ReservationManagement")]
        public async Task<IActionResult> GetStatistics(CancellationToken cancellationToken)
        {
            var result = await _service.GetStatisticsAsync(cancellationToken);

            return Ok(result);
        }
    }
}
