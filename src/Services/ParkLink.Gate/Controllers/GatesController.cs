using Microsoft.AspNetCore.Mvc;
using ParkLink.Gate.Dtos;
using ParkLink.Gate.Services.Interfaces;

namespace ParkLink.Gate.Controllers
{
    public class GatesController : BaseController
    {
        private readonly IGateService _gateService;

        public GatesController(IGateService gateService)
        {
            _gateService = gateService;
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<GateDto>> Get(
            Guid id, CancellationToken cancellationToken)
        {
            var gate = await _gateService.GetByIdAsync(id, cancellationToken);

            if (gate is null) return NotFound();

            return Ok(gate);
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<GateDto>>> Search(
            [FromQuery] GateSearchRequest request,
            CancellationToken cancellationToken)
        {
            return Ok(
                await _gateService.SearchAsync(request, cancellationToken)
            );
        }

        [HttpPost]
        public async Task<ActionResult<GateDto>> Create(
            [FromBody] CreateGateRequest request,
            CancellationToken cancellationToken)
        {
            var gate = await _gateService.CreateAsync(
                request.ParkingLotId,
                request.Name,
                request.Type,
                request.Description,
                cancellationToken);

            return CreatedAtAction(
                nameof(Get),
                new { id = gate.Id },
                gate);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<GateDto>> Update(Guid id,
            [FromBody] UpdateGateRequest request,
            CancellationToken cancellationToken)
        {
            var gate = await _gateService.UpdateAsync(id,
                request, cancellationToken);

            if (gate is null) return NotFound();

            return Ok(gate);
        }

        [HttpPatch("{id:guid}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id,
            [FromBody] UpdateGateStatusRequest request,
            CancellationToken cancellationToken)
        {
            var updated = await _gateService.UpdateStatusAsync(
                id, request.Status, cancellationToken);

            if (!updated) return NotFound();

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id,
            CancellationToken cancellationToken)
        {
            var deleted = await _gateService.DeleteAsync(id, cancellationToken);

            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}
