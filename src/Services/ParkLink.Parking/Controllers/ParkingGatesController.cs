using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkLink.Parking.Dtos.ParkingGates;
using ParkLink.Parking.Enums;
using ParkLink.Parking.Services;

namespace ParkLink.Parking.Controllers
{
    public class ParkingGatesController : BaseController
    {
        private readonly IParkingGateService _service;
        private readonly ILogger<ParkingGatesController> _logger;

        public ParkingGatesController(IParkingGateService service,
            ILogger<ParkingGatesController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetParkingGates(
            [FromQuery] ParkingGateSearchRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _service.GetParkingGatesAsync(request, cancellationToken);

            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetParkingGate(Guid id,
            CancellationToken cancellationToken)
        {
            var result = await _service.GetParkingGateByIdAsync(id, cancellationToken);

            if (result is null)
            {
                return NotFound(new
                {
                    message = $"Parking gate '{id}' was not found."
                });
            }

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = "ParkingManagement")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateParkingGate(
            [FromBody] CreateParkingGateRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _service.CreateParkingGateAsync(request, cancellationToken);

                return CreatedAtAction(nameof(GetParkingGate),
                    new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Unable to create parking gate.");

                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPut("{id:guid}")]
        [Authorize(Policy = "ParkingManagement")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateParkingGate(Guid id,
            [FromBody] UpdateParkingGateRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _service.UpdateParkingGateAsync(id,
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
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateParkingGateStatus(Guid id,
            [FromQuery] GateStatus status,
            CancellationToken cancellationToken)
        {
            try
            {
                await _service.UpdateParkingGateStatusAsync(id, status, cancellationToken);

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
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteParkingGate(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                await _service.DeleteParkingGateAsync(id, cancellationToken);

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
