using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkLink.Parking.Dtos.ParkingSlots;
using ParkLink.Parking.Enums;
using ParkLink.Parking.Services;

namespace ParkLink.Parking.Controllers
{
    public class ParkingSlotsController : BaseController
    {
        private readonly IParkingSlotService _service;
        private readonly ILogger<ParkingSlotsController> _logger;

        public ParkingSlotsController(IParkingSlotService service,
            ILogger<ParkingSlotsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetParkingSlots(
            [FromQuery] ParkingSlotSearchRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _service.GetParkingSlotsAsync(request, cancellationToken);

            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetParkingSlot(Guid id, CancellationToken cancellationToken)
        {
            var result = await _service.GetParkingSlotByIdAsync(id, cancellationToken);

            if (result is null)
            {
                return NotFound(new
                {
                    message = $"Parking slot '{id}' was not found."
                });
            }

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = "ParkingManagement")]
        public async Task<IActionResult> CreateParkingSlot(
            [FromBody] CreateParkingSlotRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _service.CreateParkingSlotAsync(request, cancellationToken);

                return CreatedAtAction(
                    nameof(GetParkingSlot),
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
                _logger.LogWarning(ex, "Unable to create parking slot.");

                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPut("{id:guid}")]
        [Authorize(Policy = "ParkingManagement")]
        public async Task<IActionResult> UpdateParkingSlot(Guid id,
            [FromBody] UpdateParkingSlotRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _service.UpdateParkingSlotAsync(id,
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
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPatch("{id:guid}/status")]
        [Authorize(Policy = "ParkingManagement")]
        public async Task<IActionResult> UpdateParkingSlotStatus(Guid id,
            [FromQuery] ParkingSlotStatus status,
            CancellationToken cancellationToken)
        {
            try
            {
                await _service.UpdateParkingSlotStatusAsync(id, status, cancellationToken);

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

        [HttpDelete("{id:guid}")]
        [Authorize(Policy = "ParkingManagement")]
        public async Task<IActionResult> DeleteParkingSlot(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                await _service.DeleteParkingSlotAsync(id, cancellationToken);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }
    }
}
