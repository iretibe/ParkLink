using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkLink.Vehicle.Dtos.Vehicles;
using ParkLink.Vehicle.Services;
using System.Security.Claims;

namespace ParkLink.Vehicle.Controllers
{
    public class VehiclesController : BaseController
    {
        private readonly IVehicleService _vehicleService;
        private readonly ILogger<VehiclesController> _logger;

        public VehiclesController(IVehicleService vehicleService, 
            ILogger<VehiclesController> logger)
        {
            _vehicleService = vehicleService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Policy = "VehicleManagement")]
        public async Task<IActionResult> GetVehicles(
            [FromQuery] VehicleSearchRequest request,
            CancellationToken cancellationToken)
        {
            var result = 
                await _vehicleService.GetVehiclesAsync(request, cancellationToken);

            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetVehicle(Guid vehicleId, 
            CancellationToken cancellationToken)
        {
            var vehicle = 
                await _vehicleService.GetVehicleByIdAsync(vehicleId, cancellationToken);

            if (vehicle == null)
            {
                return NotFound(new
                {
                    Message = $"Vehicle '{vehicleId}' was not found."
                });
            }

            return Ok(vehicle);
        }

        [HttpGet("my/{vehicleId:guid}")]
        public async Task<IActionResult> GetMyVehicles(Guid vehicleId,
            CancellationToken cancellationToken)
        {
            var ownerId = GetCurrentUserId();

            if (string.IsNullOrWhiteSpace(ownerId))
            {
                return Unauthorized();
            }

            var vehicle = await _vehicleService.GetMyVehicleAsync(Guid.Parse(ownerId), ownerId, cancellationToken);
            if (vehicle == null)
            {
                return NotFound();
            }

            return Ok(vehicle);
        }

        [HttpPost]
        public async Task<IActionResult> CreateVehicle(
            [FromBody] CreateVehicleRequest request, 
            CancellationToken cancellationToken)
        {
            var ownerId = GetCurrentUserId();
            if (string.IsNullOrWhiteSpace(ownerId))
            {
                return Unauthorized();
            }

            try
            {
                var vehicle = await _vehicleService.CreateVehicleAsync(
                        ownerId, request, cancellationToken);

                return CreatedAtAction(
                    nameof(GetVehicle), 
                        new { id = vehicle.Id }, 
                            vehicle);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    Message = ex.Message
                });
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateVehicle(Guid vehicleId,
            [FromBody] UpdateVehicleRequest request, CancellationToken cancellationToken)
        {
            var ownerId = GetCurrentUserId();
            if (string.IsNullOrWhiteSpace(ownerId))
            {
                return Unauthorized();
            }

            try
            {
                var vehicle = await _vehicleService.UpdateVehicleAsync(
                    vehicleId, ownerId, request, cancellationToken);

                return Ok(vehicle);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    Message = ex.Message
                });
            }
        }

        //[HttpPatch("{id:guid}/status")]
        //[Authorize(Policy = "VehicleManagement")]
        //public async Task<IActionResult> UpdateStatus(Guid id,
        //    [FromBody] VehicleStatus status, CancellationToken cancellationToken)
        //{
        //    try
        //    {
        //        await _vehicleService.UpdateVehicleStatusAsync(id, status, cancellationToken);

        //        return NoContent();
        //    }
        //    catch (KeyNotFoundException)
        //    {
        //        return NotFound();
        //    }
        //}

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteVehicle(Guid vehicleId,
            CancellationToken cancellationToken)
        {
            var ownerId = GetCurrentUserId();
            if (string.IsNullOrWhiteSpace(ownerId))
            {
                return Unauthorized();
            }

            try
            {
                await _vehicleService.DeleteVehicleAsync(vehicleId, ownerId, cancellationToken);

                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpPost("{vehicleId:guid}/verify")]
        [Authorize(Policy = "VehicleManagement")]
        public async Task<IActionResult> VerifyVehicle(Guid vehicleId,
            [FromBody] VehicleStatusRequest? request,
            CancellationToken cancellationToken)
        {
            var administratorId = GetCurrentUserId();
            if (string.IsNullOrWhiteSpace(administratorId))
            {
                return Unauthorized();
            }

            try
            {
                await _vehicleService.VerifyVehicleAsync(vehicleId,
                    administratorId, request, cancellationToken);

                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost("{vehicleId:guid}/suspend")]
        [Authorize(Policy = "VehicleManagement")]
        public async Task<IActionResult> SuspendVehicle(Guid vehicleId,
            [FromBody] VehicleStatusRequest? request,
            CancellationToken cancellationToken)
        {
            var administratorId = GetCurrentUserId();
            if (string.IsNullOrWhiteSpace(administratorId))
            {
                return Unauthorized();
            }

            try
            {
                await _vehicleService.SuspendVehicleAsync(vehicleId,
                    administratorId, request, cancellationToken);

                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost("{vehicleId:guid}/activate")]
        [Authorize(Policy = "VehicleManagement")]
        public async Task<IActionResult> ActivateVehicle(
            Guid vehicleId,
            [FromBody] VehicleStatusRequest? request,
            CancellationToken cancellationToken)
        {
            var administratorId = GetCurrentUserId();
            if (string.IsNullOrWhiteSpace(administratorId))
            {
                return Unauthorized();
            }

            try
            {
                await _vehicleService.ActivateVehicleAsync(vehicleId,
                    administratorId, request, cancellationToken);

                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }

        private string GetCurrentUserId()
        {
            var userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub");

            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new UnauthorizedAccessException(
                    "The authenticated user ID was not found.");
            }

            return userId;
        }
    }
}
