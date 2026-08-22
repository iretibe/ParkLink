using Microsoft.AspNetCore.Mvc;
using ParkLink.Gate.Dtos;
using ParkLink.Gate.Services.Interfaces;

namespace ParkLink.Gate.Controllers
{
    public class GateAccessController : BaseController
    {
        private readonly IGateAccessService _accessService;

        public GateAccessController(IGateAccessService accessService)
        {
            _accessService = accessService;
        }

        [HttpPost("process")]
        public async Task<ActionResult<AccessDecisionResult>> Process(
            AccessRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _accessService.ProcessAccessAsync(
                request, cancellationToken);

            return Ok(result);
        }
    }
}
