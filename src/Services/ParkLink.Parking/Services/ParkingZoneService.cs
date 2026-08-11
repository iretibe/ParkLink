using MassTransit;
using Microsoft.EntityFrameworkCore;
using ParkLink.Parking.Data;
using ParkLink.Parking.Dtos.ParkingZones;
using ParkLink.Parking.Enums;
using ParkLink.Parking.Models;
using ParkLink.Shared.Contracts.Enums;
using ParkLink.SharedKernel.Pagination;

namespace ParkLink.Parking.Services
{
    public class ParkingZoneService : IParkingZoneService
    {
        private readonly ParkingContext _context;
        private readonly IPublishEndpoint _publishEndpoint;

        public ParkingZoneService(ParkingContext context,
            IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<ParkingZoneDto> CreateParkingZoneAsync(
            CreateParkingZoneRequest request, CancellationToken cancellationToken = default)
        {
            var lotExists =
                await _context.ParkingLots.AnyAsync(
                    x => x.Id == request.ParkingLotId,
                    cancellationToken);

            if (!lotExists)
            {
                throw new KeyNotFoundException(
                    $"Parking lot '{request.ParkingLotId}' was not found.");
            }

            var entity = new ParkingZone
            {
                Id = Guid.NewGuid(),
                ParkingLotId = request.ParkingLotId,
                Name = request.Name.Trim(),
                Description = request.Description?.Trim(),
                Status = request.Status,
                Capacity = request.Capacity,
                CreatedAtUtc = DateTime.UtcNow
            };

            _context.ParkingZones.Add(entity);

            await _context.SaveChangesAsync(cancellationToken);

            await _context.Entry(entity)
                .Reference(x => x.ParkingLot)
                .LoadAsync(cancellationToken);

            return MapToDto(entity);
        }

        public async Task DeleteParkingZoneAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _context.ParkingZones
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (entity == null)
            {
                throw new KeyNotFoundException(
                    $"Parking zone '{id}' was not found.");
            }

            entity.Status = ParkingZoneStatus.Inactive;
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<ParkingZoneDto?> GetParkingZoneByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _context.ParkingZones
                .AsNoTracking()
                .Include(x => x.ParkingLot)
                .Include(x => x.Slots)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            return entity == null
                ? null
                : MapToDto(entity);
        }

        public async Task<PagedResult<ParkingZoneDto>> GetParkingZonesAsync(
            ParkingZoneSearchRequest request, CancellationToken cancellationToken = default)
        {
            var pageNumber =
                request.PageNumber <= 0
                    ? 1
                    : request.PageNumber;

            var pageSize =
                request.PageSize <= 0
                    ? 20
                    : Math.Min(request.PageSize, 100);

            var query = _context.ParkingZones
                .AsNoTracking()
                .Include(x => x.ParkingLot)
                .Include(x => x.Slots)
                .AsQueryable();

            if (request.ParkingLotId.HasValue)
            {
                query = query.Where(x =>
                    x.ParkingLotId == request.ParkingLotId.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim();

                query = query.Where(x =>
                    x.Name.Contains(search) ||
                    (x.Description != null &&
                     x.Description.Contains(search)));
            }

            if (request.Status.HasValue)
            {
                query = query.Where(x => x.Status == request.Status.Value);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var zones = await query
                .OrderBy(x => x.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<ParkingZoneDto>
            {
                Items = zones.Select(MapToDto).ToList(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<ParkingZoneDto> UpdateParkingZoneAsync(Guid id, 
            UpdateParkingZoneRequest request, CancellationToken cancellationToken = default)
        {
            var entity = await _context.ParkingZones
                .Include(x => x.ParkingLot)
                .Include(x => x.Slots)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (entity == null)
            {
                throw new KeyNotFoundException(
                    $"Parking zone '{id}' was not found.");
            }

            if (request.Capacity < entity.Slots.Count)
            {
                throw new InvalidOperationException(
                    "Parking zone capacity cannot be less than " +
                    "the number of existing parking slots.");
            }

            entity.Name = request.Name.Trim();
            entity.Description = request.Description?.Trim();
            entity.Status = request.Status;
            entity.Capacity = request.Capacity;
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return MapToDto(entity);
        }

        private static ParkingZoneDto MapToDto(ParkingZone entity)
        {
            return new ParkingZoneDto
            {
                Id = entity.Id,
                ParkingLotId = entity.ParkingLotId,
                ParkingLotName = entity.ParkingLot.Name,
                Name = entity.Name,
                Description = entity.Description,
                Status = entity.Status,
                Capacity = entity.Capacity,
                TotalSlotCount = entity.Slots.Count,
                AvailableSlotCount = entity.Slots.Count(x =>
                    x.Status == ParkingSlotStatus.Available),
                ReservedSlotCount = entity.Slots.Count(x =>
                    x.Status == ParkingSlotStatus.Reserved),
                OccupiedSlotCount = entity.Slots.Count(x =>
                    x.Status == ParkingSlotStatus.Occupied),
                MaintenanceSlotCount = entity.Slots.Count(x =>
                    x.Status == ParkingSlotStatus.Maintenance),
                DisabledSlotCount = entity.Slots.Count(x => 
                    x.Status == ParkingSlotStatus.Disabled),
                CreatedAtUtc = entity.CreatedAtUtc,
                UpdatedAtUtc = entity.UpdatedAtUtc
            };
        }
    }
}
