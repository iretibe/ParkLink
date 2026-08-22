using Microsoft.AspNetCore.Mvc;
using ParkLink.Gate.Dtos;
using ParkLink.Gate.Services.Interfaces;

namespace ParkLink.Gate.Controllers
{
    public class GateDevicesController : BaseController
    {
        private readonly IGateDeviceService _service;

        public GateDevicesController(IGateDeviceService service)
        {
            _service = service;
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<GateDeviceDto>> Get(
            Guid id, CancellationToken cancellationToken)
        {
            var device = await _service.GetByIdAsync(id, cancellationToken);

            if (device is null) return NotFound();

            return Ok(device);
        }

        [HttpGet("gate/{gateId:guid}")]
        public async Task<ActionResult<IReadOnlyCollection<GateDeviceDto>>> GetForGate(
            Guid gateId, CancellationToken cancellationToken)
        {
            return Ok(
                await _service.GetForGateAsync(gateId, cancellationToken)
            );
        }

        [HttpPost]
        public async Task<ActionResult<GateDeviceDto>> Register(
            RegisterGateDeviceRequest request,
            CancellationToken cancellationToken)
        {
            var device = await _service.RegisterAsync(request, cancellationToken);

            return CreatedAtAction(
                nameof(Get),
                new { id = device.Id },
                device);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<GateDeviceDto>> Update(Guid id,
            UpdateGateDeviceRequest request,
            CancellationToken cancellationToken)
        {
            var device = await _service.UpdateAsync(id, request, cancellationToken);

            if (device is null) return NotFound();

            return Ok(device);
        }
    }
}
