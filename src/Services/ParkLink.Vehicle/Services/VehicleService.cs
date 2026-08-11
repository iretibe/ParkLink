using MassTransit;
using Microsoft.EntityFrameworkCore;
using ParkLink.ServiceDefaults.Correlation;
using ParkLink.SharedKernel.Events.Vehicle;
using ParkLink.SharedKernel.Pagination;
using ParkLink.Vehicle.Data;
using ParkLink.Vehicle.Dtos.Vehicles;
using ParkLink.Vehicle.Enums;
using ParkLink.Vehicle.Models;
using System.Diagnostics;

namespace ParkLink.Vehicle.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly VehicleContext _context;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ICorrelationContext _correlationContext;
        private readonly ILogger<VehicleService> _logger;

        public VehicleService(VehicleContext context,
            IPublishEndpoint publishEndpoint,
            ICorrelationContext correlationContext,
            ILogger<VehicleService> logger)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
            _correlationContext = correlationContext;
            _logger = logger;
        }

        public async Task ActivateVehicleAsync(Guid vehicleId, 
            string administratorId, VehicleStatusRequest? request = null, 
            CancellationToken cancellationToken = default)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync(cancellationToken);

            var vehicle = await GetVehicleEntityAsync(vehicleId, cancellationToken);

            if (vehicle.Status != VehicleStatus.Suspended)
            {
                throw new InvalidOperationException(
                    "Only suspended vehicles can be activated.");
            }

            var oldStatus = vehicle.Status;

            vehicle.Status = VehicleStatus.Verified;
            vehicle.IsActive = true;
            vehicle.StatusReason = request?.Reason;
            vehicle.UpdatedAtUtc = DateTime.UtcNow;

            await _publishEndpoint.Publish(
                new VehicleStatusChangedIntegrationEvent(
                    vehicle.Id, vehicle.OwnerId,
                    vehicle.Status.ToString(), 
                    VehicleStatus.Verified.ToString(),                     
                    administratorId, request?.Reason),
                cancellationToken
            );

            await _context.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation(
                "Vehicle {VehicleId} activated by administrator {AdministratorId}",
                vehicle.Id,
                administratorId);
        }

        public async Task<VehicleDetailsDto> CreateVehicleAsync(string ownerId, 
            CreateVehicleRequest request, CancellationToken cancellationToken = default)
        {
            var existingVehicle = await _context.Vehicles
                .FirstOrDefaultAsync(x => 
                    x.LicensePlateNumber == request.LicensePlateNumber,
                    cancellationToken);

            if (existingVehicle != null)
            {
                throw new InvalidOperationException(
                    "A vehicle with this license plate already exists.");
            }

            if (!string.IsNullOrWhiteSpace(request.VIN))
            {
                var vin = request.VIN.Trim().ToUpperInvariant();

                var vinExists =
                    await _context.Vehicles.AnyAsync(x => x.VIN == vin, cancellationToken);

                if (vinExists)
                {
                    throw new InvalidOperationException(
                        "A vehicle with this VIN already exists.");
                }
            }

            await using var transaction =
                await _context.Database.BeginTransactionAsync(cancellationToken);

            var vehicle = new Models.Vehicle
            {
                Id = Guid.NewGuid(),
                OwnerId = ownerId,
                LicensePlateNumber = request.LicensePlateNumber.Trim().ToUpperInvariant(),
                VIN = request.VIN?.Trim().ToUpperInvariant(),
                Make = request.Make.Trim(),
                Model = request.Model.Trim(),
                Year = request.Year,
                Color = request.Color?.Trim(),
                VehicleType = request.VehicleType,
                Status = VehicleStatus.Pending,
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow
            };
                        
            _context.Vehicles.Add(vehicle);

            if (request.Documents != null)
            {
                foreach (var document in request.Documents)
                {
                    vehicle.Documents.Add(
                        new VehicleDocument
                        {
                            Id = Guid.NewGuid(),
                            VehicleId = vehicle.Id,
                            DocumentType = document.DocumentType,
                            DocumentNumber = document.DocumentNumber,
                            IssuingCountryCode = document.IssuingCountryCode,
                            DocumentUrl = document.DocumentUrl,
                            ExpiryDateUtc = document.ExpiryDateUtc,
                            CreatedAtUtc = DateTime.UtcNow
                        }
                    );
                }
            }

            await _publishEndpoint.Publish(
                new VehicleCreatedIntegrationEvent(
                vehicle.Id, vehicle.OwnerId,
                vehicle.LicensePlateNumber, vehicle.Make, vehicle.Model,
                vehicle.VehicleType.ToString()),            

            cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation(
                "Vehicle {VehicleId} created for owner {OwnerId}",
                vehicle.Id,
                ownerId);

            return MapToDetailsDto(vehicle);
        }

        public async Task DeleteVehicleAsync(Guid vehicleId, string ownerId, 
            CancellationToken cancellationToken = default)
        {
            var vehicle = await _context.Vehicles.FirstOrDefaultAsync(x => 
                x.Id == vehicleId && x.OwnerId == ownerId, cancellationToken);

            if (vehicle == null)
            {
                throw new KeyNotFoundException(
                    $"Vehicle '{vehicleId}' was not found.");
            }

            await using var transaction =
                await _context.Database.BeginTransactionAsync(cancellationToken);

            vehicle.IsActive = false;
            vehicle.Status = VehicleStatus.Suspended;
            vehicle.StatusReason = "Vehicle removed by owner.";
            vehicle.UpdatedAtUtc = DateTime.UtcNow;

            await _publishEndpoint.Publish(
                new VehicleDeletedIntegrationEvent(
                    vehicle.Id, vehicle.OwnerId,
                    vehicle.LicensePlateNumber), 
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation(
                "Vehicle {VehicleId} deleted by owner {OwnerId}",
                vehicleId,
                ownerId);
        }

        public async Task<VehicleDetailsDto?> GetMyVehicleAsync(Guid vehicleId, string ownerId, CancellationToken cancellationToken = default)
        {
            var vehicle = await _context.Vehicles
                .AsNoTracking()
                .Include(x => x.Documents)
                .FirstOrDefaultAsync(
                    x => x.Id == vehicleId && x.OwnerId == ownerId, cancellationToken);

            return vehicle == null
                ? null
                : MapToDetailsDto(vehicle);
        }

        public async Task<VehicleDetailsDto?> GetVehicleByIdAsync(Guid vehicleId, CancellationToken cancellationToken = default)
        {
            var vehicle = await _context.Vehicles
                .AsNoTracking()
                .Include(x => x.Documents)
                .FirstOrDefaultAsync(x => x.Id == vehicleId, cancellationToken);

            return vehicle == null
                ? null
                : MapToDetailsDto(vehicle);
        }

        public async Task<PagedResult<VehicleListItemDto>> GetVehiclesAsync(
            VehicleSearchRequest request, CancellationToken cancellationToken = default)
        {
            var pageNumber = request.PageNumber <= 0
                ? 1
                : request.PageNumber;

            var pageSize = request.PageSize <= 0
                ? 20
                : Math.Min(request.PageSize, 100);

            var query = _context.Vehicles
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim();

                query = query.Where(x =>
                    x.LicensePlateNumber.Contains(search) ||
                    (x.VIN != null && x.VIN.Contains(search)) ||
                    x.Make.Contains(search) ||
                    x.Model.Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(request.OwnerId))
            {
                query = query.Where(x =>
                    x.OwnerId == request.OwnerId);
            }

            if (request.Status.HasValue)
            {
                query = query.Where(x =>
                    x.Status == request.Status.Value);
            }

            if (request.VehicleType.HasValue)
            {
                query = query.Where(x =>
                    x.VehicleType == request.VehicleType.Value);
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(x =>
                    x.IsActive == request.IsActive.Value);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var vehicles =
                await query
                    .OrderByDescending(x => x.CreatedAtUtc)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(x => new VehicleListItemDto
                    {
                        Id = x.Id,
                        OwnerId = x.OwnerId,
                        LicensePlateNumber = x.LicensePlateNumber,
                        VIN = x.VIN,
                        Make = x.Make,
                        Model = x.Model,
                        Year = x.Year,
                        Color = x.Color,
                        VehicleType = x.VehicleType,
                        Status = x.Status,
                        IsActive = x.IsActive,
                        CreatedAtUtc = x.CreatedAtUtc
                    })
                    .ToListAsync(cancellationToken);

            return new PagedResult<VehicleListItemDto>
            {
                Items = vehicles,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task SuspendVehicleAsync(Guid vehicleId, 
            string administratorId, VehicleStatusRequest? request = null, 
            CancellationToken cancellationToken = default)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync(cancellationToken);

            var vehicle = await GetVehicleEntityAsync(vehicleId, cancellationToken);

            var oldStatus = vehicle.Status;

            vehicle.Status = VehicleStatus.Suspended;
            vehicle.IsActive = false;
            vehicle.SuspendedAtUtc = DateTime.UtcNow;
            vehicle.SuspendedByUserId = administratorId;
            vehicle.StatusReason = request?.Reason;
            vehicle.UpdatedAtUtc = DateTime.UtcNow;

            await _publishEndpoint.Publish(
                new VehicleStatusChangedIntegrationEvent(
                    vehicle.Id, vehicle.OwnerId,
                    oldStatus.ToString(), VehicleStatus.Suspended.ToString(),
                    administratorId, request?.Reason),
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation(
                "Vehicle {VehicleId} suspended by administrator {AdministratorId}",
                vehicle.Id,
                administratorId);
        }

        public async Task<VehicleDetailsDto> UpdateVehicleAsync(Guid vehicleId, 
            string ownerId, UpdateVehicleRequest request, CancellationToken cancellationToken = default)
        {
            var vehicle = await _context.Vehicles
                    .Include(x => x.Documents)
                    .FirstOrDefaultAsync(
                        x => x.Id == vehicleId && x.OwnerId == ownerId,
                        cancellationToken);

            if (vehicle == null)
            {
                throw new KeyNotFoundException(
                    $"Vehicle '{vehicleId}' was not found.");
            }

            var licensePlate =
                request.LicensePlateNumber.Trim().ToUpperInvariant();

            var duplicatePlate =await _context.Vehicles.AnyAsync(x => 
                    x.Id != vehicleId && x.LicensePlateNumber == licensePlate,
                    cancellationToken);

            if (duplicatePlate)
            {
                throw new InvalidOperationException(
                    "A vehicle with this license plate already exists.");
            }

            await using var transaction =
                await _context.Database.BeginTransactionAsync(cancellationToken);

            vehicle.LicensePlateNumber = licensePlate;
            vehicle.VIN = request.VIN?.Trim().ToUpperInvariant();
            vehicle.Make = request.Make.Trim();
            vehicle.Model = request.Model.Trim();
            vehicle.Year = request.Year;
            vehicle.Color = request.Color?.Trim();
            vehicle.VehicleType = request.VehicleType;
            vehicle.UpdatedAtUtc = DateTime.UtcNow;

            await _publishEndpoint.Publish(
               new VehicleUpdatedIntegrationEvent(
                   vehicle.Id, vehicle.OwnerId, vehicle.LicensePlateNumber, 
                   vehicle.Make, vehicle.Model),
               cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation("Vehicle {VehicleId} updated.", vehicleId);

            return MapToDetailsDto(vehicle);
        }

        public async Task VerifyVehicleAsync(Guid vehicleId, 
            string administratorId, VehicleStatusRequest? request = null, 
            CancellationToken cancellationToken = default)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync(cancellationToken);

            var vehicle = await GetVehicleEntityAsync(vehicleId, cancellationToken);

            vehicle.Status = VehicleStatus.Verified;
            vehicle.IsActive = true;
            vehicle.VerifiedAtUtc = DateTime.UtcNow;
            vehicle.VerifiedByUserId = administratorId;
            vehicle.StatusReason = request?.Reason;
            vehicle.UpdatedAtUtc = DateTime.UtcNow;

            await _publishEndpoint.Publish(
                new VehicleStatusChangedIntegrationEvent(
                    vehicle.Id, vehicle.OwnerId, VehicleStatus.Pending.ToString(),
                    vehicle.Status.ToString(), administratorId, request?.Reason), 
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation(
                "Vehicle {VehicleId} verified by administrator {AdministratorId}",
                vehicle.Id,
                administratorId);
        }

        private async Task<Models.Vehicle> GetVehicleEntityAsync(
            Guid vehicleId, CancellationToken cancellationToken)
        {
            var vehicle =
                await _context.Vehicles
                    .FirstOrDefaultAsync(
                        x => x.Id == vehicleId, cancellationToken);

            if (vehicle == null)
            {
                throw new KeyNotFoundException(
                    $"Vehicle '{vehicleId}' was not found.");
            }

            return vehicle;
        }

        private static VehicleDetailsDto MapToDetailsDto(Models.Vehicle vehicle)
        {
            return new VehicleDetailsDto
            {
                Id = vehicle.Id,
                OwnerId = vehicle.OwnerId,
                LicensePlateNumber = vehicle.LicensePlateNumber,
                VIN = vehicle.VIN,
                Make = vehicle.Make,
                Model = vehicle.Model,
                Year = vehicle.Year,
                Color = vehicle.Color,
                VehicleType = vehicle.VehicleType,
                Status = vehicle.Status,
                IsActive = vehicle.IsActive,
                CreatedAtUtc = vehicle.CreatedAtUtc,
                UpdatedAtUtc = vehicle.UpdatedAtUtc,
                VerifiedAtUtc = vehicle.VerifiedAtUtc,
                VerifiedByUserId = vehicle.VerifiedByUserId,
                SuspendedAtUtc = vehicle.SuspendedAtUtc,
                SuspendedByUserId = vehicle.SuspendedByUserId,
                StatusReason = vehicle.StatusReason,

                Documents = vehicle.Documents
                    .Select(x => new Dtos.Documents.VehicleDocumentDto
                    {
                        Id = x.Id,
                        DocumentType = x.DocumentType,
                        DocumentNumber = x.DocumentNumber,
                        IssuingCountryCode = x.IssuingCountryCode,
                        DocumentUrl = x.DocumentUrl,
                        ExpiryDateUtc = x.ExpiryDateUtc,
                        CreatedAtUtc = x.CreatedAtUtc
                    })
                    .ToList()
            };
        }
    }
}
