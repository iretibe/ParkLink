using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkLink.SharedKernel.Pagination;
using ParkLink.Users.Authorization;
using ParkLink.Users.Dtos.Drivers;
using ParkLink.Users.Services;

namespace ParkLink.Users.Controllers
{
    public class DriversController : BaseController
    {
        private readonly IDriverService _driverService;

        public DriversController(IDriverService driverService)
        {
            _driverService = driverService;
        }

        // Get all drivers with optional search and pagination
        [HttpGet]
        [Authorize(Policy = ParkLinkPolicies.DriverManagement)]
        [ProducesResponseType(typeof(PagedResult<DriverListItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDrivers([FromQuery] DriverSearchRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _driverService.GetDriversAsync(request, cancellationToken);

            return Ok(result);
        }

        // Get driver details by ID
        [HttpGet("{id}")]
        [Authorize(Policy = ParkLinkPolicies.DriverManagement)]
        [ProducesResponseType(typeof(DriverDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDriver(string id, CancellationToken cancellationToken)
        {
            var driver = await _driverService.GetDriverByIdAsync(id, cancellationToken);
            if (driver == null)
            {
                return NotFound(new
                {
                    message = "Driver not found."
                });
            }

            return Ok(driver);
        }

        // Approve a driver
        [HttpPost("{id}/approve")]
        [Authorize(Policy = ParkLinkPolicies.DriverManagement)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> ApproveDriver(string id,
            [FromBody] DriverActionRequest? request, CancellationToken cancellationToken)
        {
            try
            {
                await _driverService.ApproveDriverAsync(id, request, cancellationToken);

                return Ok(new
                {
                    message = "Driver approved successfully."
                });
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

        // Reject a driver
        [HttpPost("{id}/reject")]
        [Authorize(Policy = ParkLinkPolicies.DriverManagement)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> RejectDriver(string id,
            [FromBody] DriverActionRequest? request, CancellationToken cancellationToken)
        {
            try
            {
                await _driverService.RejectDriverAsync(id, request, cancellationToken);

                return Ok(new
                {
                    message = "Driver rejected successfully."
                });
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

        // Suspend a driver
        [HttpPost("{id}/suspend")]
        [Authorize(Policy = ParkLinkPolicies.DriverManagement)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> SuspendDriver(string id,
            [FromBody] DriverActionRequest? request, CancellationToken cancellationToken)
        {
            try
            {
                await _driverService.SuspendDriverAsync(id, request, cancellationToken);

                return Ok(new
                {
                    message = "Driver suspended successfully."
                });
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
