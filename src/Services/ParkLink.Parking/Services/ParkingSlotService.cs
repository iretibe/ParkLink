using MassTransit;
using Microsoft.EntityFrameworkCore;
using ParkLink.Parking.Data;
using ParkLink.Parking.Dtos.ParkingSlots;
using ParkLink.Parking.Enums;
using ParkLink.Parking.Exceptions;
using ParkLink.Parking.Models;
using ParkLink.Shared.Contracts.Enums;
using ParkLink.SharedKernel.Events.Parking;
using ParkLink.SharedKernel.Pagination;

namespace ParkLink.Parking.Services
{
    public class ParkingSlotService : IParkingSlotService
    {
        private readonly ParkingContext _context;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<ParkingSlotService> _logger;

        public ParkingSlotService(ParkingContext context, 
            IPublishEndpoint publishEndpoint, 
            ILogger<ParkingSlotService> logger)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task<ParkingSlotDto> CreateParkingSlotAsync(
            CreateParkingSlotRequest request, CancellationToken cancellationToken = default)
        {
            var zone = await _context.ParkingZones
                .AsNoTracking()
                .Include(x => x.ParkingLot)
                .FirstOrDefaultAsync(x => 
                    x.Id == request.ParkingZoneId, cancellationToken);

            if (zone == null)
            {
                throw new KeyNotFoundException(
                    $"Parking zone '{request.ParkingZoneId}' was not found.");
            }

            if (zone.Status != ParkingZoneStatus.Active)
            {
                throw new InvalidOperationException(
                    "A parking slot cannot be created in an inactive parking zone.");
            }

            var currentSlotCount = await _context.ParkingSlots
                .CountAsync(x => x.ParkingZoneId == request.ParkingZoneId && x.IsActive,
                cancellationToken);

            if (currentSlotCount >= zone.Capacity)
            {
                throw new InvalidOperationException(
                    "The parking zone has reached its configured capacity.");
            }

            var duplicate =
                await _context.ParkingSlots.AnyAsync(
                    x =>
                        x.ParkingZoneId == request.ParkingZoneId &&
                        x.SlotNumber == request.SlotNumber.Trim(), cancellationToken);

            if (duplicate)
            {
                throw new InvalidOperationException(
                    $"Slot '{request.SlotNumber}' already exists in this zone.");
            }

            var entity = new ParkingSlot
            {
                Id = Guid.NewGuid(),
                ParkingZoneId = request.ParkingZoneId,
                SlotNumber = request.SlotNumber.Trim(),
                SlotType = request.SlotType,
                Status = ParkingSlotStatus.Available,
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow
            };

            _context.ParkingSlots.Add(entity);

            // MassTransit EF Core transactional bus outbox.
            await _publishEndpoint.Publish(
                new ParkingSlotCreatedIntegrationEvent(
                    entity.Id, 
                    entity.ParkingZoneId, 
                    zone.ParkingLotId,
                    entity.SlotNumber, 
                    entity.SlotType.ToString(),
                    entity.Status.ToString()
                ), cancellationToken
            );

            // The lot occupancy changes because a new active slot was created.
            await PublishOccupancyChangedAsync(zone.ParkingLotId, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Parking slot {SlotId} created in zone {ParkingZoneId}.",
                entity.Id, entity.ParkingZoneId);

            entity.ParkingZone = zone;

            return MapToDto(entity);
        }

        public async Task DeleteParkingSlotAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _context.ParkingSlots
                .Include(x => x.ParkingZone)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (entity == null)
            {
                throw new KeyNotFoundException(
                    $"Parking slot '{id}' was not found.");
            }

            if (entity.Status == ParkingSlotStatus.Occupied)
            {
                throw new InvalidOperationException(
                    "An occupied parking slot cannot be deleted.");
            }

            var parkingLotId = await _context.ParkingZones
                .Where(x => x.Id == entity.ParkingZoneId)
                .Select(x => x.ParkingLotId)
                .FirstAsync(cancellationToken);

            entity.IsActive = false;
            entity.Status = ParkingSlotStatus.Disabled;
            entity.UpdatedAtUtc = DateTime.UtcNow;
            entity.LastStatusChangedAtUtc = DateTime.UtcNow;

            await _publishEndpoint.Publish(
                new ParkingSlotStatusChangedIntegrationEvent(
                    entity.Id,
                    entity.ParkingZoneId,
                    parkingLotId,
                    entity.SlotNumber,
                    ParkingSlotStatus.Disabled.ToString(),
                    ParkingSlotStatus.Disabled.ToString(),
                    entity.LastStatusChangedAtUtc.Value),
                cancellationToken);

            await PublishOccupancyChangedAsync(parkingLotId, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<ParkingSlotDto?> GetParkingSlotByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _context.ParkingSlots
                .AsNoTracking()
                .Include(x => x.ParkingZone)
                    .ThenInclude(x => x.ParkingLot)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            return entity == null ? null : MapToDto(entity);
        }

        public async Task<PagedResult<ParkingSlotDto>> GetParkingSlotsAsync(
            ParkingSlotSearchRequest request, CancellationToken cancellationToken = default)
        {
            var pageNumber =
            request.PageNumber <= 0
                ? 1
                : request.PageNumber;

            var pageSize =
                request.PageSize <= 0
                    ? 20
                    : Math.Min(request.PageSize, 100);

            var query = _context.ParkingSlots
                .AsNoTracking()
                .Include(x => x.ParkingZone)
                    .ThenInclude(x => x.ParkingLot)
                .AsQueryable();

            if (request.ParkingZoneId.HasValue)
            {
                query = query.Where(x =>
                    x.ParkingZoneId == request.ParkingZoneId.Value);
            }

            if (request.ParkingLotId.HasValue)
            {
                query = query.Where(x =>
                    x.ParkingZone.ParkingLotId == request.ParkingLotId.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim();

                query = query.Where(x => 
                    x.SlotNumber.Contains(search));
            }

            if (request.SlotType.HasValue)
            {
                query = query.Where(x => 
                    x.SlotType == request.SlotType.Value);
            }

            if (request.Status.HasValue)
            {
                query = query.Where(x =>
                    x.Status == request.Status.Value);
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(x =>
                    x.IsActive == request.IsActive.Value);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var slots = await query
                .OrderBy(x => x.SlotNumber)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<ParkingSlotDto>
            {
                Items = slots.Select(MapToDto).ToList(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<ParkingSlotDto> UpdateParkingSlotAsync(Guid id, 
            UpdateParkingSlotRequest request, CancellationToken cancellationToken = default)
        {
            var entity = await _context.ParkingSlots
                .Include(x => x.ParkingZone)
                .ThenInclude(x => x.ParkingLot)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (entity == null)
            {
                throw new KeyNotFoundException(
                    $"Parking slot '{id}' was not found.");
            }

            if (!string.IsNullOrWhiteSpace(request.SlotNumber))
            {
                var slotNumber = request.SlotNumber.Trim();

                var duplicate = await _context.ParkingSlots
                    .AnyAsync(x =>
                        x.Id != id &&
                        x.ParkingZoneId == entity.ParkingZoneId &&
                        x.SlotNumber == slotNumber,
                    cancellationToken);

                if (duplicate)
                {
                    throw new InvalidOperationException(
                        $"Slot '{slotNumber}' already exists in this zone.");
                }

                entity.SlotNumber = slotNumber;
            }

            entity.SlotNumber = request.SlotNumber.Trim();
            entity.SlotType = request.SlotType;
            entity.IsActive = request.IsActive;
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _publishEndpoint.Publish(
                new ParkingSlotUpdatedIntegrationEvent(
                    entity.Id,
                    entity.ParkingZoneId,
                    entity.SlotNumber,
                    entity.SlotType.ToString(),
                    entity.Status.ToString(),
                    entity.IsActive,
                    entity.UpdatedAtUtc.Value),
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return MapToDto(entity);
        }

        public async Task UpdateParkingSlotStatusAsync(Guid id, 
            ParkingSlotStatus status, CancellationToken cancellationToken = default)
        {
            var entity = await _context.ParkingSlots
                .Include(x => x.ParkingZone)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (entity == null)
            {
                throw new KeyNotFoundException(
                    $"Parking slot '{id}' was not found.");
            }

            if (entity.Status == status)
            {
                return;
            }

            var previousStatus = entity.Status;

            entity.Status = status;
            entity.LastStatusChangedAtUtc = DateTime.UtcNow;
            entity.UpdatedAtUtc = DateTime.UtcNow;

            var parkingLotId = await _context.ParkingZones
                .Where(x => x.Id == entity.ParkingZoneId)
                .Select(x => x.ParkingLotId)
                .FirstAsync(cancellationToken);

            await _publishEndpoint.Publish(
                new ParkingSlotStatusChangedIntegrationEvent(
                    entity.Id, entity.ParkingZoneId,
                    parkingLotId, 
                    entity.SlotNumber, 
                    entity.Status.ToString(), 
                    status.ToString(),
                    entity.LastStatusChangedAtUtc.GetValueOrDefault()
                ), cancellationToken
            );

            await PublishOccupancyChangedAsync(parkingLotId, cancellationToken);

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogWarning(
                    ex,
                    "Concurrency conflict while updating parking slot {SlotId}.",
                    id);

                throw new ParkingConcurrencyException(
                    $"Parking slot '{id}' was modified by another request. " +
                    "Please reload the slot and try again.",
                    ex);
            }

            _logger.LogInformation(
               "Parking slot {SlotId} changed status from {PreviousStatus} to {NewStatus}.",
               id, previousStatus, status);
        }

        private async Task PublishOccupancyChangedAsync(Guid parkingLotId,  CancellationToken cancellationToken)
        {
            var slots = await _context.ParkingSlots
                .AsNoTracking()
                .Where(x => x.ParkingZone.ParkingLotId == parkingLotId && x.IsActive)
                .Select(x => new
                {
                    x.Status
                })
                .ToListAsync(cancellationToken);

            var totalSlots = slots.Count;

            var availableSlots = slots.Count(x => 
                x.Status == ParkingSlotStatus.Available);

            var reservedSlots = slots.Count(x => 
                x.Status == ParkingSlotStatus.Reserved);

            var occupiedSlots = slots.Count(x => 
                x.Status == ParkingSlotStatus.Occupied);

            var maintenanceSlots = slots.Count(x => 
                x.Status == ParkingSlotStatus.Maintenance);

            var disabledSlots = slots.Count(x => 
                x.Status == ParkingSlotStatus.Disabled);

            var calculatedAtUtc = DateTime.UtcNow;

            await _publishEndpoint.Publish(
                new ParkingLotOccupancyChangedIntegrationEvent(
                    ParkingLotId: parkingLotId,
                    TotalSlots: totalSlots,
                    AvailableSlots: availableSlots,
                    ReservedSlots: reservedSlots,
                    OccupiedSlots: occupiedSlots,
                    MaintenanceSlots: maintenanceSlots,
                    DisabledSlots: disabledSlots,
                    CalculatedAtUtc: calculatedAtUtc
                ), cancellationToken
            );
        }

        private static ParkingSlotDto MapToDto(ParkingSlot entity)
        {
            return new ParkingSlotDto
            {
                Id = entity.Id,
                ParkingZoneId = entity.ParkingZoneId,
                ParkingLotId = entity.ParkingZone.ParkingLotId,
                ParkingLotName = entity.ParkingZone.ParkingLot.Name,
                ParkingZoneName = entity.ParkingZone.Name,
                SlotNumber = entity.SlotNumber,
                SlotType = entity.SlotType,
                Status = entity.Status,
                IsActive = entity.IsActive,
                CreatedAtUtc = entity.CreatedAtUtc,
                UpdatedAtUtc = entity.UpdatedAtUtc,
                LastStatusChangedAtUtc = entity.LastStatusChangedAtUtc
            };
        }
    }
}
