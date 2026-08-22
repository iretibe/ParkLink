using Microsoft.EntityFrameworkCore;
using ParkLink.Gate.Data;
using ParkLink.Gate.Dtos;
using ParkLink.Gate.Enums;
using ParkLink.Gate.Services.Interfaces;

namespace ParkLink.Gate.Services.Implementations
{
    public sealed class GateService : IGateService
    {
        private readonly GateContext _context;

        public GateService(GateContext context)
        {
            _context = context;
        }

        public async Task<GateDto> CreateAsync(Guid parkingLotId, string name, 
            GateType type, string? description, CancellationToken cancellationToken = default)
        {
            var exists = await _context.Gates.AnyAsync(
            x => x.ParkingLotId == parkingLotId &&
                x.Name == name.Trim(), cancellationToken
            );

            if (exists)
            {
                throw new InvalidOperationException(
                    "A gate with this name already exists in the parking lot.");
            }

            var gate = Entities.Gate.Create(parkingLotId, 
                name, type, description);

            _context.Gates.Add(gate);

            await _context.SaveChangesAsync(cancellationToken);

            return (await GetByIdAsync(gate.Id, cancellationToken))!;
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var gate = await _context.Gates
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (gate is null) return false;

            _context.Gates.Remove(gate);

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<GateDto?> GetByIdAsync(Guid id, 
            CancellationToken cancellationToken = default)
        {
            return await _context.Gates
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new GateDto(
                    x.Id,
                    x.ParkingLotId,
                    x.Name,
                    x.Type,
                    x.Status,
                    x.Description,
                    x.CreatedAtUtc,
                    x.UpdatedAtUtc,
                    x.Devices.Count))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<PaginatedResult<GateDto>> SearchAsync(GateSearchRequest request, 
            CancellationToken cancellationToken = default)
        {
            var page = Math.Max(1, request.Page);
            var pageSize = Math.Clamp(request.PageSize, 1, 100);

            var query = _context.Gates
                .AsNoTracking()
                .AsQueryable();

            if (request.ParkingLotId.HasValue)
            {
                query = query.Where(x =>
                    x.ParkingLotId == request.ParkingLotId.Value);
            }

            if (request.Type.HasValue)
            {
                query = query.Where(x =>
                    x.Type == request.Type.Value);
            }

            if (request.Status.HasValue)
            {
                query = query.Where(x =>
                    x.Status == request.Status.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim();

                query = query.Where(x =>
                    x.Name.Contains(search) ||
                    (x.Description != null &&
                     x.Description.Contains(search))
                );
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderBy(x => x.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new GateDto(
                    x.Id,
                    x.ParkingLotId,
                    x.Name,
                    x.Type,
                    x.Status,
                    x.Description,
                    x.CreatedAtUtc,
                    x.UpdatedAtUtc,
                    x.Devices.Count))
                .ToListAsync(cancellationToken);

            var totalPages =
                (int)Math.Ceiling(totalCount / (double)pageSize);

            return new PaginatedResult<GateDto>(
                items,
                page,
                pageSize,
                totalCount,
                totalPages
            );
        }

        public async Task<GateDto?> UpdateAsync(Guid id, 
            UpdateGateRequest request, CancellationToken cancellationToken = default)
        {
            var gate = await _context.Gates
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (gate is null)
                return null;

            gate.Update(request.Name, request.Description);

            await _context.SaveChangesAsync(cancellationToken);

            return await GetByIdAsync(id, cancellationToken);
        }

        public async Task<bool> UpdateStatusAsync(Guid id, GateStatus status, 
            CancellationToken cancellationToken = default)
        {
            var gate = await _context.Gates
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (gate is null)
                return false;

            gate.SetStatus(status);

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
