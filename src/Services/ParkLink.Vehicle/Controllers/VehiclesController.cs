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

        // GET: api/vehicles
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

        // GET: api/vehicles/{vehicleId}
        [HttpGet("{vehicleId:guid}")]
        public async Task<IActionResult> GetVehicle(
            Guid vehicleId, CancellationToken cancellationToken)
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

        // GET: api/vehicles/my/{vehicleId}
        [HttpGet("my/{vehicleId:guid}")]
        public async Task<IActionResult> GetMyVehicle(Guid vehicleId,
            CancellationToken cancellationToken)
        {
            var ownerId = GetCurrentUserId();

            var vehicle = await _vehicleService.GetMyVehicleAsync(
                vehicleId, ownerId, cancellationToken);

            if (vehicle == null)
            {
                return NotFound(new
                {
                    Message = $"Vehicle '{vehicleId}' was not found."
                });
            }

            return Ok(vehicle);
        }

        // POST: api/vehicles
        [HttpPost]
        public async Task<IActionResult> CreateVehicle(
            [FromBody] CreateVehicleRequest request,
            CancellationToken cancellationToken)
        {
            var ownerId = GetCurrentUserId();

            try
            {
                var vehicle = await _vehicleService.CreateVehicleAsync(
                    ownerId, request, cancellationToken);

                return CreatedAtAction(
                    nameof(GetVehicle),
                        new { vehicleId = vehicle.Id },
                            vehicle
                );
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex,
                    "Unable to create vehicle for owner {OwnerId}.",
                    ownerId
                );

                return Conflict(new
                {
                    Message = ex.Message
                });
            }
        }

        // PUT: api/vehicles/{vehicleId}
        [HttpPut("{vehicleId:guid}")]
        public async Task<IActionResult> UpdateVehicle(Guid vehicleId,
            [FromBody] UpdateVehicleRequest request, CancellationToken cancellationToken)
        {
            var ownerId = GetCurrentUserId();

            try
            {
                var vehicle = await _vehicleService.UpdateVehicleAsync(
                    vehicleId, ownerId, request, cancellationToken);

                return Ok(vehicle);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new
                {
                    Message = $"Vehicle '{vehicleId}' was not found."
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(
                    ex,
                    "Unable to update vehicle {VehicleId}.",
                    vehicleId
                );

                return Conflict(new
                {
                    Message = ex.Message
                });
            }
        }

        // PATCH: api/vehicles/{vehicleId}/status
        /*
        [HttpPatch("{vehicleId:guid}/status")]
        [Authorize(Policy = "VehicleManagement")]
        public async Task<IActionResult> UpdateStatus(
            Guid vehicleId,
            [FromBody] VehicleStatus status,
            CancellationToken cancellationToken)
        {
            try
            {
                await _vehicleService.UpdateVehicleStatusAsync(
                    vehicleId,
                    status,
                    cancellationToken);

                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new
                {
                    Message = $"Vehicle '{vehicleId}' was not found."
                });
            }
        }
        */

        // DELETE: api/vehicles/{vehicleId}
        [HttpDelete("{vehicleId:guid}")]
        public async Task<IActionResult> DeleteVehicle(Guid vehicleId,
            CancellationToken cancellationToken)
        {
            var ownerId = GetCurrentUserId();

            try
            {
                await _vehicleService.DeleteVehicleAsync(vehicleId, ownerId, cancellationToken);

                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new
                {
                    Message = $"Vehicle '{vehicleId}' was not found."
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        // POST: api/vehicles/{vehicleId}/verify
        [HttpPost("{vehicleId:guid}/verify")]
        [Authorize(Policy = "VehicleManagement")]
        public async Task<IActionResult> VerifyVehicle(Guid vehicleId,
            [FromBody] VehicleStatusRequest? request,
            CancellationToken cancellationToken)
        {
            var administratorId = GetCurrentUserId();

            try
            {
                await _vehicleService.VerifyVehicleAsync(vehicleId,
                    administratorId, request, cancellationToken);

                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new
                {
                    Message = $"Vehicle '{vehicleId}' was not found."
                });
            }
        }

        // POST: api/vehicles/{vehicleId}/suspend
        [HttpPost("{vehicleId:guid}/suspend")]
        [Authorize(Policy = "VehicleManagement")]
        public async Task<IActionResult> SuspendVehicle(Guid vehicleId,
            [FromBody] VehicleStatusRequest? request,
            CancellationToken cancellationToken)
        {
            var administratorId = GetCurrentUserId();

            try
            {
                await _vehicleService.SuspendVehicleAsync(vehicleId,
                    administratorId, request, cancellationToken);

                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new
                {
                    Message = $"Vehicle '{vehicleId}' was not found."
                });
            }
        }

        // POST: api/vehicles/{vehicleId}/activate
        [HttpPost("{vehicleId:guid}/activate")]
        [Authorize(Policy = "VehicleManagement")]
        public async Task<IActionResult> ActivateVehicle(
            Guid vehicleId,
            [FromBody] VehicleStatusRequest? request,
            CancellationToken cancellationToken)
        {
            var administratorId = GetCurrentUserId();

            try
            {
                await _vehicleService.ActivateVehicleAsync(vehicleId,
                    administratorId, request, cancellationToken);

                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new
                {
                    Message = $"Vehicle '{vehicleId}' was not found."
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(
                    ex,
                    "Unable to activate vehicle {VehicleId}.",
                    vehicleId);

                return Conflict(new
                {
                    Message = ex.Message
                });
            }
        }

        private string GetCurrentUserId()
        {
            var userId =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new UnauthorizedAccessException(
                    "The authenticated user ID was not found.");
            }

            return userId;
        }
    }
}
