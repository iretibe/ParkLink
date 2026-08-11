using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkLink.Parking.Dtos.ParkingZones;
using ParkLink.Parking.Services;

namespace ParkLink.Parking.Controllers
{
    public class ParkingZonesController : BaseController
    {
        private readonly IParkingZoneService _service;
        private readonly ILogger<ParkingZonesController> _logger;

        public ParkingZonesController(IParkingZoneService service,
            ILogger<ParkingZonesController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetParkingZones(
            [FromQuery] ParkingZoneSearchRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _service.GetParkingZonesAsync(request, cancellationToken);

            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetParkingZone(Guid id, CancellationToken cancellationToken)
        {
            var result = await _service.GetParkingZoneByIdAsync(id, cancellationToken);

            if (result is null)
            {
                return NotFound(new
                {
                    message = $"Parking zone '{id}' was not found."
                });
            }

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = "ParkingManagement")]
        public async Task<IActionResult> CreateParkingZone(
            [FromBody] CreateParkingZoneRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _service.CreateParkingZoneAsync(request, cancellationToken);

                return CreatedAtAction(
                    nameof(GetParkingZone),
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
        [Authorize(Policy = "ParkingManagement")]
        public async Task<IActionResult> UpdateParkingZone(Guid id,
            [FromBody] UpdateParkingZoneRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _service.UpdateParkingZoneAsync(id,
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
        public async Task<IActionResult> DeleteParkingZone(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                await _service.DeleteParkingZoneAsync(id, cancellationToken);

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
