using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkLink.Parking.Dtos.ParkingLots;
using ParkLink.Parking.Services;

namespace ParkLink.Parking.Controllers
{
    public class ParkingLotsController : BaseController
    {
        private readonly IParkingLotService _service;
        private readonly ILogger<ParkingLotsController> _logger;

        public ParkingLotsController(IParkingLotService service,
            ILogger<ParkingLotsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetParkingLots(
            [FromQuery] ParkingLotSearchRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _service.GetParkingLotsAsync(request, cancellationToken);

            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetParkingLot(Guid id, CancellationToken cancellationToken)
        {
            var result = await _service.GetParkingLotByIdAsync(id, cancellationToken);

            if (result is null)
            {
                return NotFound(new
                {
                    message = $"Parking lot '{id}' was not found."
                });
            }

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = "ParkingManagement")]
        public async Task<IActionResult> CreateParkingLot(
            [FromBody] CreateParkingLotRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _service.CreateParkingLotAsync(request, cancellationToken);

                return CreatedAtAction(
                    nameof(GetParkingLot),
                    new { id = result.Id },
                    result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Unable to create parking lot.");

                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPut("{id:guid}")]
        [Authorize(Policy = "ParkingManagement")]
        public async Task<IActionResult> UpdateParkingLot(Guid id,
            [FromBody] UpdateParkingLotRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _service.UpdateParkingLotAsync(id,
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

        [HttpDelete("{id:guid}")]
        [Authorize(Policy = "ParkingManagement")]
        public async Task<IActionResult> DeleteParkingLot(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                await _service.DeleteParkingLotAsync(id, cancellationToken);

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
