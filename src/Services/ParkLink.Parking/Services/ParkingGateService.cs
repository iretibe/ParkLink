using MassTransit;
using Microsoft.EntityFrameworkCore;
using ParkLink.Parking.Data;
using ParkLink.Parking.Dtos.ParkingGates;
using ParkLink.Parking.Enums;
using ParkLink.Parking.Models;
using ParkLink.SharedKernel.Events.Parking;
using ParkLink.SharedKernel.Pagination;

namespace ParkLink.Parking.Services
{
    public class ParkingGateService : IParkingGateService
    {
        private readonly ParkingContext _context;
        private readonly IPublishEndpoint _publishEndpoint;

        public ParkingGateService(ParkingContext context, 
            IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<ParkingGateDto> CreateParkingGateAsync(CreateParkingGateRequest request, CancellationToken cancellationToken = default)
        {
            var lot = await _context.ParkingLots
                .FirstOrDefaultAsync(x => 
                    x.Id == request.ParkingLotId && x.IsActive, cancellationToken);

            if (lot == null)
            {
                throw new KeyNotFoundException(
                    $"Parking lot '{request.ParkingLotId}' was not found.");
            }

            var entity = new ParkingGate
            {
                Id = Guid.NewGuid(),
                ParkingLotId = request.ParkingLotId,
                Name = request.Name.Trim(),
                GateType = request.GateType,
                Status = GateStatus.Offline,
                DeviceIdentifier = request.DeviceIdentifier?.Trim(),
                RfidReaderIdentifier = request.RfidReaderIdentifier?.Trim(),
                OcrCameraIdentifier = request.OcrCameraIdentifier?.Trim(),
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow
            };

            _context.ParkingGates.Add(entity);

            await _context.SaveChangesAsync(cancellationToken);

            await _publishEndpoint.Publish(
                new ParkingGateCreatedIntegrationEvent(
                    entity.Id, entity.ParkingLotId, entity.Name,
                    entity.GateType.ToString(), entity.Status.ToString(),
                    entity.DeviceIdentifier, entity.RfidReaderIdentifier, 
                    entity.OcrCameraIdentifier
                ), cancellationToken
            );

            entity.ParkingLot = lot;

            return MapToDto(entity);
        }

        public async Task DeleteParkingGateAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _context.ParkingGates
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (entity == null)
            {
                throw new KeyNotFoundException(
                    $"Parking gate '{id}' was not found.");
            }

            entity.IsActive = false;
            entity.Status = GateStatus.Disabled;
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<ParkingGateDto?> GetParkingGateByIdAsync(Guid id, 
            CancellationToken cancellationToken = default)
        {
            var entity = await _context.ParkingGates
                .AsNoTracking()
                .Include(x => x.ParkingLot)
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

            return entity == null
                ? null
                : MapToDto(entity);
        }

        public async Task<PagedResult<ParkingGateDto>> GetParkingGatesAsync(
            ParkingGateSearchRequest request, CancellationToken cancellationToken = default)
        {
            var pageNumber =
                request.PageNumber <= 0
                    ? 1
                    : request.PageNumber;

            var pageSize =
                request.PageSize <= 0
                    ? 20
                    : Math.Min(request.PageSize, 100);

            var query = _context.ParkingGates
                .AsNoTracking()
                .Include(x => x.ParkingLot)
                .AsQueryable();

            if (request.ParkingLotId.HasValue)
            {
                query = query.Where(x =>
                    x.ParkingLotId == request.ParkingLotId.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var value = request.Search.Trim();

                query = query.Where(x =>
                    x.Name.Contains(value) ||
                    (x.DeviceIdentifier != null &&
                     x.DeviceIdentifier.Contains(value)) ||
                    (x.RfidReaderIdentifier != null &&
                     x.RfidReaderIdentifier.Contains(value)) ||
                    (x.OcrCameraIdentifier != null &&
                     x.OcrCameraIdentifier.Contains(value)));
            }

            if (request.Status.HasValue)
            {
                query = query.Where(x => x.Status == request.Status.Value);
            }

            if (request.GateType.HasValue)
            {
                query = query.Where(x => x.GateType == request.GateType.Value);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var gates = await query
                .OrderBy(x => x.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<ParkingGateDto>
            {
                Items = gates.Select(MapToDto).ToList(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<ParkingGateDto> UpdateParkingGateAsync(Guid id, 
            UpdateParkingGateRequest request, CancellationToken cancellationToken = default)
        {
            var entity = await _context.ParkingGates
                .Include(x => x.ParkingLot)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (entity == null)
            {
                throw new KeyNotFoundException(
                    $"Parking gate '{id}' was not found.");
            }

            entity.Name = request.Name.Trim();
            entity.GateType = request.GateType;
            entity.Status = request.Status;
            entity.DeviceIdentifier = request.DeviceIdentifier?.Trim();
            entity.RfidReaderIdentifier = request.RfidReaderIdentifier?.Trim();
            entity.OcrCameraIdentifier = request.OcrCameraIdentifier?.Trim();
            entity.IsActive = request.IsActive;
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return MapToDto(entity);
        }

        public async Task UpdateParkingGateStatusAsync(Guid gateId, 
            GateStatus status, CancellationToken cancellationToken = default)
        {
            var gate = await _context.ParkingGates
                .FirstOrDefaultAsync(x => x.Id == gateId, cancellationToken);

            if (gate == null)
            {
                throw new KeyNotFoundException(
                    $"Parking gate '{gateId}' was not found.");
            }

            var previousStatus = gate.Status;

            if (previousStatus == status)
            {
                return;
            }

            gate.Status = status;
            gate.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            await _publishEndpoint.Publish(
                new ParkingGateStatusChangedIntegrationEvent(
                    gate.Id, gate.ParkingLotId, gate.Status.ToString(), 
                    status.ToString(), DateTime.UtcNow
                ), cancellationToken
            );
        }

        private static ParkingGateDto MapToDto(ParkingGate entity)
        {
            return new ParkingGateDto
            {
                Id = entity.Id,
                ParkingLotId = entity.ParkingLotId,
                ParkingLotName = entity.ParkingLot.Name,
                Name = entity.Name,
                GateType = entity.GateType,
                Status = entity.Status,
                DeviceIdentifier = entity.DeviceIdentifier,
                RfidReaderIdentifier = entity.RfidReaderIdentifier,
                OcrCameraIdentifier = entity.OcrCameraIdentifier,
                IsActive = entity.IsActive,
                CreatedAtUtc = entity.CreatedAtUtc,
                UpdatedAtUtc = entity.UpdatedAtUtc
            };
        }
    }
}
