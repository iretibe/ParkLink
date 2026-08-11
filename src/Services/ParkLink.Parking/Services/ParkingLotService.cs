using MassTransit;
using Microsoft.EntityFrameworkCore;
using ParkLink.Parking.Data;
using ParkLink.Parking.Dtos.ParkingLots;
using ParkLink.Parking.Enums;
using ParkLink.Parking.Models;
using ParkLink.Shared.Contracts.Enums;
using ParkLink.SharedKernel.Events.Parking;
using ParkLink.SharedKernel.Pagination;

namespace ParkLink.Parking.Services
{
    public class ParkingLotService : IParkingLotService
    {
        private readonly ParkingContext _context;
        private readonly IPublishEndpoint _publishEndpoint;

        public ParkingLotService(ParkingContext context, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<ParkingLotDetailsDto> CreateParkingLotAsync(
            CreateParkingLotRequest request,
            CancellationToken cancellationToken = default)
        {
            var entity = new ParkingLot
            {
                Id = Guid.NewGuid(),
                Name = request.Name.Trim(),
                Description = request.Description?.Trim(),
                CountryCode = request.CountryCode.Trim().ToUpperInvariant(),
                City = request.City.Trim(),
                Address = request.Address?.Trim(),
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Status = ParkingLotStatus.Draft,
                IsActive = false,
                CreatedAtUtc = DateTime.UtcNow
            };

            _context.ParkingLots.Add(entity);

            await _context.SaveChangesAsync(cancellationToken);

            await _publishEndpoint.Publish(
                new ParkingLotCreatedIntegrationEvent(
                    entity.Id, entity.Name, entity.CountryCode, 
                    entity.City, entity.Address, entity.Latitude, 
                    entity.Longitude
                ), cancellationToken
            );

            return MapToDetailsDto(entity);
        }

        public async Task DeleteParkingLotAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _context.ParkingLots
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (entity == null)
            {
                throw new KeyNotFoundException(
                    $"Parking lot '{id}' was not found.");
            }

            // Soft deletion for parking infrastructure.
            entity.IsActive = false;
            entity.Status = ParkingLotStatus.Inactive;
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<ParkingLotDetailsDto?> GetParkingLotByIdAsync(Guid id,
            CancellationToken cancellationToken = default)
        {
            var lot = await _context.ParkingLots
                .AsNoTracking()
                .Include(x => x.Zones)
                    .ThenInclude(x => x.Slots)
                .Include(x => x.Gates)
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

            return lot == null
                ? null
                : MapToDetailsDto(lot);
        }

        public async Task<PagedResult<ParkingLotDetailsDto>> GetParkingLotsAsync(
            ParkingLotSearchRequest request,
            CancellationToken cancellationToken = default)
        {
            var pageNumber = request.PageNumber <= 0
            ? 1
            : request.PageNumber;

            var pageSize = request.PageSize <= 0
                ? 20
                : Math.Min(request.PageSize, 100);

            var query = _context.ParkingLots
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim();

                query = query.Where(x =>
                    x.Name.Contains(search) ||
                    (x.Description != null &&
                     x.Description.Contains(search)) ||
                    x.City.Contains(search) ||
                    x.CountryCode.Contains(search) ||
                    (x.Address != null &&
                     x.Address.Contains(search)));
            }

            if (!string.IsNullOrWhiteSpace(request.CountryCode))
            {
                var countryCode = request.CountryCode.Trim();

                query = query.Where(x => x.CountryCode == countryCode);
            }

            if (!string.IsNullOrWhiteSpace(request.City))
            {
                var city = request.City.Trim();

                query = query.Where(x => x.City == city);
            }

            if (request.Status.HasValue)
            {
                query = query.Where(x => x.Status == request.Status.Value);
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(x => x.IsActive == request.IsActive.Value);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var lots = await query
                .Include(x => x.Zones)
                    .ThenInclude(x => x.Slots)
                .Include(x => x.Gates)
                .OrderBy(x => x.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var items = lots
                .Select(MapToDetailsDto)
                .ToList();

            return new PagedResult<ParkingLotDetailsDto>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<ParkingLotDetailsDto> UpdateParkingLotAsync(Guid id,
            UpdateParkingLotRequest request, CancellationToken cancellationToken = default)
        {
            var entity = await _context.ParkingLots
                .Include(x => x.Zones)
                    .ThenInclude(x => x.Slots)
                .Include(x => x.Gates)
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

            if (entity == null)
            {
                throw new KeyNotFoundException(
                    $"Parking lot '{id}' was not found.");
            }

            entity.Name = request.Name.Trim();
            entity.Description = request.Description?.Trim();
            entity.CountryCode = request.CountryCode.Trim().ToUpperInvariant();
            entity.City = request.City.Trim();
            entity.Address = request.Address?.Trim();
            entity.Latitude = request.Latitude;
            entity.Longitude = request.Longitude;
            entity.Status = request.Status;
            entity.IsActive = request.IsActive;
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            await _publishEndpoint.Publish(
                new ParkingLotUpdatedIntegrationEvent(
                    entity.Id, entity.Name, entity.CountryCode, 
                    entity.City, entity.Address, entity.Latitude, 
                    entity.Longitude, entity.Status.ToString()
                ), cancellationToken
            );

            return MapToDetailsDto(entity);
        }

        private static ParkingLotDetailsDto MapToDetailsDto(ParkingLot entity)
        {
            var slots = entity.Zones
                .SelectMany(x => x.Slots)
                .ToList();

            return new ParkingLotDetailsDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                CountryCode = entity.CountryCode,
                City = entity.City,
                Address = entity.Address,
                Latitude = entity.Latitude,
                Longitude = entity.Longitude,
                Status = entity.Status,
                IsActive = entity.IsActive,
                CreatedAtUtc = entity.CreatedAtUtc,
                UpdatedAtUtc = entity.UpdatedAtUtc,

                ZoneCount = entity.Zones.Count,
                GateCount = entity.Gates.Count,

                TotalSlotCount = slots.Count,

                AvailableSlotCount =
                    slots.Count(x =>
                        x.Status == ParkingSlotStatus.Available),

                ReservedSlotCount =
                    slots.Count(x =>
                        x.Status == ParkingSlotStatus.Reserved),

                OccupiedSlotCount =
                    slots.Count(x =>
                        x.Status == ParkingSlotStatus.Occupied),

                MaintenanceSlotCount =
                    slots.Count(x =>
                        x.Status == ParkingSlotStatus.Maintenance),

                DisabledSlotCount =
                    slots.Count(x =>
                        x.Status == ParkingSlotStatus.Disabled),

                Zones = entity.Zones
                    .Select(x => new ParkingZoneSummaryDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Status = x.Status,
                        Capacity = x.Capacity,
                        TotalSlotCount = x.Slots.Count,
                        AvailableSlotCount =
                            x.Slots.Count(s =>
                                s.Status == ParkingSlotStatus.Available),
                        OccupiedSlotCount =
                            x.Slots.Count(s =>
                                s.Status == ParkingSlotStatus.Occupied)
                    })
                    .ToList(),

                Gates = entity.Gates
                    .Select(x => new ParkingGateSummaryDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        GateType = x.GateType,
                        Status = x.Status,
                        IsActive = x.IsActive,
                        DeviceIdentifier =
                            x.DeviceIdentifier,
                        RfidReaderIdentifier =
                            x.RfidReaderIdentifier,
                        OcrCameraIdentifier =
                            x.OcrCameraIdentifier
                    })
                    .ToList()
            };
        }
    }
}
