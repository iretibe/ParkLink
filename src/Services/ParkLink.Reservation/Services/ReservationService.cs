using MassTransit;
using Microsoft.EntityFrameworkCore;
using ParkLink.Reservation.Data;
using ParkLink.Reservation.Dtos;
using ParkLink.Reservation.Enums;
using ParkLink.Reservation.Models;
using ParkLink.Shared.Contracts.Enums;
using ParkLink.SharedKernel.Events.Parking;
using ParkLink.SharedKernel.Events.Reservation;
using ParkLink.SharedKernel.Pagination;

namespace ParkLink.Reservation.Services
{
    public class ReservationService : IReservationService
    {
        private readonly ReservationContext _context;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<ReservationService> _logger;

        public ReservationService(ReservationContext context,
            IPublishEndpoint publishEndpoint,
            ILogger<ReservationService> logger)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task<ReservationDetailsDto> ActivateReservationAsync(
            Guid reservationId, CancellationToken cancellationToken = default)
        {
            var reservation = await GetReservationAsync(reservationId, cancellationToken);

            if (reservation.Status != ReservationStatus.Confirmed)
            {
                throw new InvalidOperationException(
                    $"Reservation cannot be activated from {reservation.Status} status.");
            }

            reservation.Status = ReservationStatus.Active;
            reservation.ActualEntryTimeUtc = DateTime.UtcNow;
            reservation.UpdatedAtUtc = DateTime.UtcNow;

            await _publishEndpoint.Publish(
                new ReservationActivatedIntegrationEvent(
                    reservation.Id,
                    reservation.ReservationNumber,
                    reservation.UserId,
                    reservation.VehicleId,
                    reservation.ParkingLotId,
                    reservation.ParkingZoneId,
                    reservation.ParkingSlotId,
                    reservation.ActualEntryTimeUtc.Value),
                cancellationToken
            );

            await _context.SaveChangesAsync(cancellationToken);

            return MapToDetailsDto(reservation);
        }

        public async Task CancelReservationAsync(Guid reservationId, 
            string userId, CancelReservationRequest request, 
            CancellationToken cancellationToken = default)
        {
            var reservation = await GetOwnedReservationAsync(reservationId,
                userId, cancellationToken);

            if (reservation.Status is
                ReservationStatus.Completed or ReservationStatus.Cancelled)
            {
                throw new InvalidOperationException(
                    $"Reservation cannot be cancelled while in {reservation.Status} status.");
            }

            reservation.Status = ReservationStatus.Cancelled;
            reservation.CancellationReason = request.Reason.Trim();
            reservation.CancelledAtUtc = DateTime.UtcNow;
            reservation.CancelledByUserId = userId;
            reservation.UpdatedAtUtc = DateTime.UtcNow;

            await _publishEndpoint.Publish(
                new ReservationCancelledIntegrationEvent(
                    reservation.Id,
                    reservation.ReservationNumber,
                    reservation.UserId,
                    reservation.VehicleId,
                    reservation.ParkingLotId,
                    reservation.ParkingZoneId,
                    reservation.ParkingSlotId,
                    reservation.CancellationReason,
                    reservation.CancelledByUserId,
                    reservation.CancelledAtUtc
                ),
                cancellationToken
            );

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<ReservationAvailabilityDto> CheckAvailabilityAsync(
            ReservationAvailabilityRequest request, CancellationToken cancellationToken = default)
        {
            ValidateReservationDates(request.StartTimeUtc, request.EndTimeUtc);

            var query = _context.Reservations
                .AsNoTracking()
                .Where(x =>
                    x.ParkingLotId == request.ParkingLotId &&
                    x.StartTimeUtc < request.EndTimeUtc &&
                    x.EndTimeUtc > request.StartTimeUtc &&
                    x.Status != ReservationStatus.Cancelled &&
                    x.Status != ReservationStatus.Expired &&
                    x.Status != ReservationStatus.NoShow);

            if (request.ParkingZoneId.HasValue)
            {
                query = query
                    .Where(x => x.ParkingZoneId == request.ParkingZoneId.Value);
            }

            var reservations = await query.ToListAsync(cancellationToken);

            return new ReservationAvailabilityDto
            {
                ReservedSlots = reservations
                    .Select(x => x.ParkingSlotId)
                    .Distinct()
                    .Count()
            };
        }

        public async Task<ReservationDetailsDto> CheckInAsync(Guid reservationId, 
            CheckInRequest request, CancellationToken cancellationToken = default)
        {
            var reservation = await GetReservationAsync(reservationId, cancellationToken);

            if (reservation.Status != ReservationStatus.Confirmed)
            {
                throw new InvalidOperationException(
                    "Only confirmed reservations can be checked in.");
            }

            var enteredAtUtc = DateTime.UtcNow;

            reservation.Status = ReservationStatus.Active;
            reservation.ActualEntryTimeUtc = DateTime.UtcNow;
            reservation.UpdatedAtUtc = enteredAtUtc;

            await _context.SaveChangesAsync(cancellationToken);

            await _publishEndpoint.Publish(
                new VehicleEnteredParkingLotIntegrationEvent(
                    reservation.Id,
                    reservation.ParkingLotId,
                    reservation.ParkingZoneId,
                    reservation.ParkingSlotId,
                    request.ParkingGateId,
                    reservation.UserId,                    
                    reservation.VehicleId,
                    request.LicensePlateNumber,
                    reservation.ReservationNumber,
                    request.RfidTag,
                    request.OcrPlateNumber,
                    reservation.UpdatedAtUtc),
                cancellationToken
            );

            return MapToDetailsDto(reservation);
        }

        public async Task<ReservationDetailsDto> CheckOutAsync(Guid reservationId, 
            CheckOutRequest request, CancellationToken cancellationToken = default)
        {
            var reservation = await GetReservationAsync(reservationId, cancellationToken);

            if (reservation.Status != ReservationStatus.Active)
            {
                throw new InvalidOperationException(
                    "Only active reservations can be checked out.");
            }

            var exitedAtUtc = DateTime.UtcNow;

            reservation.Status = ReservationStatus.Completed;
            reservation.ActualExitTimeUtc = DateTime.UtcNow;
            reservation.UpdatedAtUtc = exitedAtUtc;

            await _context.SaveChangesAsync(cancellationToken);

            await _publishEndpoint.Publish(
                new VehicleExitedParkingLotIntegrationEvent(
                    reservation.Id,
                    reservation.ParkingLotId,
                    reservation.ParkingZoneId,
                    reservation.ParkingSlotId,
                    request.ParkingGateId,
                    reservation.VehicleId,
                    reservation.UserId,
                    request.LicensePlateNumber,
                    reservation.ReservationNumber,
                    request.RfidTag,
                    request.OcrPlateNumber,
                    reservation.UpdatedAtUtc),
                cancellationToken
            );

            return MapToDetailsDto(reservation);
        }

        public async Task<ReservationDetailsDto> CompleteReservationAsync(
            Guid reservationId, CancellationToken cancellationToken = default)
        {
            var reservation = await GetReservationAsync(reservationId, cancellationToken);

            if (reservation.Status != ReservationStatus.Active)
            {
                throw new InvalidOperationException(
                    $"Reservation cannot be completed from {reservation.Status} status.");
            }

            reservation.Status = ReservationStatus.Completed;
            reservation.ActualExitTimeUtc = DateTime.UtcNow;
            reservation.UpdatedAtUtc = DateTime.UtcNow;
            
            await _publishEndpoint.Publish(
                new ReservationCompletedIntegrationEvent(
                    reservation.Id,
                    reservation.ReservationNumber,
                    reservation.UserId,
                    reservation.VehicleId,
                    reservation.ParkingLotId,
                    reservation.ParkingZoneId,
                    reservation.ParkingSlotId,
                    reservation.ActualEntryTimeUtc!.Value,
                    reservation.ActualExitTimeUtc.Value,
                    reservation.Amount,
                    reservation.CurrencyCode),
                cancellationToken
            );

            await _context.SaveChangesAsync(cancellationToken);

            return MapToDetailsDto(reservation);
        }

        public async Task<ReservationDetailsDto> ConfirmReservationAsync(
            Guid reservationId, CancellationToken cancellationToken = default)
        {
            var reservation = await GetReservationAsync(reservationId, cancellationToken);

            if (reservation.Status != ReservationStatus.Pending &&
                reservation.Status != ReservationStatus.Held)
            {
                throw new InvalidOperationException(
                    $"Reservation cannot be confirmed from {reservation.Status} status.");
            }

            reservation.Status = ReservationStatus.Confirmed;
            reservation.PaymentStatus = ReservationPaymentStatus.Paid;
            reservation.UpdatedAtUtc = DateTime.UtcNow;

            await _publishEndpoint.Publish(
                new ReservationConfirmedIntegrationEvent(
                    reservation.Id,
                    reservation.ReservationNumber,
                    reservation.UserId,
                    reservation.VehicleId,
                    reservation.ParkingLotId,
                    reservation.ParkingZoneId,
                    reservation.ParkingSlotId,
                    reservation.StartTimeUtc,
                    reservation.EndTimeUtc,
                    reservation.Amount,
                    reservation.CurrencyCode,
                    reservation.PaymentReference,
                    reservation.AccessCredential,
                    reservation.AccessMethod.ToString()),
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return MapToDetailsDto(reservation);
        }

        public async Task<ReservationHoldDto> CreateHoldAsync(string userId, 
            CreateReservationHoldRequest request, CancellationToken cancellationToken = default)
        {
            if (request.HoldMinutes <= 0)
            {
                request.HoldMinutes = 15;
            }

            var expiresAt = DateTime.UtcNow.AddMinutes(request.HoldMinutes);

            var existingHold = await _context.ReservationHolds
                .FirstOrDefaultAsync(
                    x =>
                        x.ParkingSlotId ==
                        request.ParkingSlotId &&
                        x.Status ==
                        ReservationHoldStatus.Active &&
                        x.ExpiresAtUtc > DateTime.UtcNow,
                    cancellationToken
                );

            if (existingHold != null)
            {
                throw new InvalidOperationException(
                    "The parking slot is currently held.");
            }

            var hold = new ReservationHold
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ParkingSlotId = request.ParkingSlotId,
                Status = ReservationHoldStatus.Active,
                ExpiresAtUtc = expiresAt,
                CreatedAtUtc = DateTime.UtcNow
            };

            _context.ReservationHolds.Add(hold);

            await _publishEndpoint.Publish(
                new ReservationHoldCreatedIntegrationEvent(
                    hold.Id,
                    hold.ReservationId,
                    hold.UserId,
                    hold.Reservation.VehicleId,
                    hold.Reservation.ParkingLotId,
                    hold.Reservation.ParkingZoneId,
                    hold.ParkingSlotId,
                    hold.Reservation.ParkingLotName,
                    hold.Reservation.ReservationNumber,
                    hold.CreatedAtUtc,
                    hold.ExpiresAtUtc
                ),
                cancellationToken
            );

            await _context.SaveChangesAsync(cancellationToken);

            return new ReservationHoldDto
            {
                Id = hold.Id,
                ParkingSlotId = hold.ParkingSlotId,
                VehicleId = request.VehicleId,
                Status = hold.Status,
                ExpiresAtUtc = hold.ExpiresAtUtc,
                CreatedAtUtc = hold.CreatedAtUtc
            };
        }

        public async Task<ReservationDetailsDto> CreateReservationAsync(string userId, 
            CreateReservationRequest request, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(userId);

            ValidateReservationDates(request.StartTimeUtc, request.EndTimeUtc);

            var available = await IsSlotAvailableAsync(
                request.ParkingSlotId,
                request.StartTimeUtc,
                request.EndTimeUtc,
                null,
                cancellationToken);

            if (!available)
            {
                throw new InvalidOperationException(
                    "The selected parking slot is not available for the requested period.");
            }

            var now = DateTime.UtcNow;

            var reservation = new Models.Reservation
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ReservationNumber = GenerateReservationNumber(now),
                VehicleId = request.VehicleId,
                ParkingLotId = request.ParkingLotId,
                ParkingZoneId = request.ParkingZoneId,
                ParkingSlotId = request.ParkingSlotId,
                ReservationType = request.ReservationType,
                AccessMethod = request.AccessMethod,
                StartTimeUtc = request.StartTimeUtc,
                EndTimeUtc = request.EndTimeUtc,
                Status = ReservationStatus.Pending,
                PaymentStatus = ReservationPaymentStatus.Pending,
                Amount = CalculateAmount(request),
                CurrencyCode = request.CurrencyCode.Trim().ToUpperInvariant(),
                CreatedAtUtc = now
            };

            _context.Reservations.Add(reservation);

            await _publishEndpoint.Publish(
                new ReservationCreatedIntegrationEvent(
                    reservation.Id,
                    reservation.ReservationNumber,
                    reservation.UserId,
                    reservation.VehicleId,
                    reservation.ParkingLotId,
                    reservation.ParkingZoneId,
                    reservation.ParkingSlotId,
                    reservation.ParkingLotName,
                    reservation.StartTimeUtc,
                    reservation.EndTimeUtc,
                    reservation.Amount,
                    reservation.CurrencyCode),
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Reservation {ReservationId} created with number {ReservationNumber}.",
                reservation.Id,
                reservation.ReservationNumber);

            return MapToDetailsDto(reservation);
        }

        public async Task ExpireReservationAsync(Guid reservationId, 
            CancellationToken cancellationToken = default)
        {
            var reservation = await GetReservationAsync(reservationId, cancellationToken);

            if (reservation.Status is
                ReservationStatus.Completed or ReservationStatus.Cancelled)
            {
                return;
            }

            reservation.Status = ReservationStatus.Expired;
            reservation.ExpiredAtUtc = DateTime.UtcNow;
            reservation.UpdatedAtUtc = DateTime.UtcNow;

            await _publishEndpoint.Publish(
                new ReservationExpiredIntegrationEvent(
                    reservation.Id,
                    reservation.ReservationNumber,
                    reservation.UserId,
                    reservation.VehicleId,
                    reservation.ParkingLotId,
                    reservation.ParkingZoneId,
                    reservation.ParkingSlotId,
                    reservation.ExpiredAtUtc
                ),
                cancellationToken
            );

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<ReservationDetailsDto> ExtendReservationAsync(
            Guid reservationId, string userId, ExtendReservationRequest request, 
            CancellationToken cancellationToken = default)
        {
            var reservation = await GetOwnedReservationAsync(reservationId,
                userId, cancellationToken);

            if (reservation.Status is ReservationStatus.Completed or
                ReservationStatus.Cancelled or ReservationStatus.Expired or
                ReservationStatus.NoShow)
            {
                throw new InvalidOperationException(
                    $"Reservation cannot be extended while in {reservation.Status} status.");
            }

            if (request.NewEndTimeUtc <= reservation.EndTimeUtc)
            {
                throw new InvalidOperationException(
                    "The new end time must be later than the current end time.");
            }

            var available = await IsSlotAvailableAsync(
                reservation.ParkingSlotId, reservation.EndTimeUtc, 
                request.NewEndTimeUtc, reservation.Id, cancellationToken);

            if (!available)
            {
                throw new InvalidOperationException(
                    "The parking slot is not available for the requested extension period.");
            }

            var previousEndTime = reservation.EndTimeUtc;

            reservation.EndTimeUtc = request.NewEndTimeUtc;
            reservation.Amount = CalculateAmount(reservation.StartTimeUtc,
                reservation.EndTimeUtc, reservation.ReservationType);
            reservation.UpdatedAtUtc = DateTime.UtcNow;

            await _publishEndpoint.Publish(
                new ReservationExtendedIntegrationEvent(
                    reservation.Id,
                    reservation.ReservationNumber,
                    reservation.UserId,
                    reservation.ParkingLotId,
                    reservation.ParkingZoneId,
                    reservation.ParkingSlotId,
                    previousEndTime,
                    reservation.EndTimeUtc,
                    reservation.Amount,
                    reservation.CurrencyCode),
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return MapToDetailsDto(reservation);
        }

        public async Task<ReservationDetailsDto?> GetMyReservationAsync(
            Guid reservationId, string userId, CancellationToken cancellationToken = default)
        {
            var reservation = await _context.Reservations
                .AsNoTracking()
                .Include(x => x.Hold)
                .FirstOrDefaultAsync(x => x.Id == reservationId 
                    && x.UserId == userId, cancellationToken);

            return reservation == null
                ? null
                : MapToDetailsDto(reservation);
        }

        public async Task<PagedResult<ReservationListItemDto>> GetMyReservationsAsync(
            string userId, ReservationSearchRequest request, CancellationToken cancellationToken = default)
        {
            request ??= new ReservationSearchRequest();

            request.UserId = userId;

            return await GetReservationsAsync(request, cancellationToken);
        }

        public async Task<ReservationDetailsDto?> GetReservationByIdAsync(
            Guid reservationId, CancellationToken cancellationToken = default)
        {
            var reservation = await _context.Reservations
                .AsNoTracking()
                .Include(x => x.Hold)
                .FirstOrDefaultAsync(x => x.Id == reservationId, cancellationToken);

            return reservation == null
                ? null
                : MapToDetailsDto(reservation);
        }

        public async Task<PagedResult<ReservationListItemDto>> GetReservationsAsync(
            ReservationSearchRequest request, CancellationToken cancellationToken = default)
        {
            var pageNumber = request.PageNumber <= 0
                ? 1
                : request.PageNumber;

            var pageSize = request.PageSize <= 0
                ? 20
                : Math.Min(request.PageSize, 100);

            var query = BuildSearchQuery(request);

            var totalCount =
                await query.CountAsync(cancellationToken);

            var reservations = await query
                .OrderByDescending(x => x.CreatedAtUtc)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return new PagedResult<ReservationListItemDto>
            {
                Items = reservations
                    .Select(MapToListItemDto)
                    .ToList(),

                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<ReservationStatisticsDto> GetStatisticsAsync(
            CancellationToken cancellationToken = default)
        {
            var reservations = await _context.Reservations
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var result = new ReservationStatisticsDto
            {
                TotalReservations = reservations.Count,

                Pending = reservations.Count(
                    x => x.Status == ReservationStatus.Pending),

                Held = reservations.Count(
                    x => x.Status == ReservationStatus.Held),

                Confirmed = reservations.Count(
                    x => x.Status == ReservationStatus.Confirmed),

                Active = reservations.Count(
                    x => x.Status == ReservationStatus.Active),

                Completed = reservations.Count(
                    x => x.Status == ReservationStatus.Completed),

                Cancelled = reservations.Count(
                    x => x.Status == ReservationStatus.Cancelled),

                Expired = reservations.Count(
                    x => x.Status == ReservationStatus.Expired),

                NoShows = reservations.Count(
                    x => x.Status == ReservationStatus.NoShow),

                TotalRevenue = reservations
                    .Where(x =>
                        x.PaymentStatus ==
                        ReservationPaymentStatus.Paid)
                    .Sum(x => x.Amount)
            };

            result.AverageReservationValue =
                result.TotalReservations == 0
                    ? 0
                    : reservations.Average(x => x.Amount);

            return result;
        }

        public async Task MarkReservationAsNoShowAsync(Guid reservationId, CancellationToken cancellationToken = default)
        {
            var reservation = await GetReservationAsync(reservationId, cancellationToken);

            if (reservation.Status != ReservationStatus.Confirmed)
            {
                throw new InvalidOperationException(
                    $"Reservation cannot be marked as no-show from {reservation.Status} status.");
            }

            reservation.Status = ReservationStatus.NoShow;
            reservation.NoShowAtUtc = DateTime.UtcNow;
            reservation.UpdatedAtUtc = DateTime.UtcNow;

            await _publishEndpoint.Publish(
                new ReservationNoShowIntegrationEvent(
                    reservation.Id,
                    reservation.ReservationNumber,
                    reservation.UserId,
                    reservation.VehicleId,
                    reservation.ParkingLotId,
                    reservation.ParkingZoneId,
                    reservation.ParkingSlotId,
                    reservation.NoShowAtUtc),
                cancellationToken
            );

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task ReleaseHoldAsync(Guid holdId, string userId, 
            CancellationToken cancellationToken = default)
        {
            var hold = await _context.ReservationHolds
                .FirstOrDefaultAsync(x => x.Id == holdId && x.UserId == userId, cancellationToken);

            if (hold == null)
            {
                throw new KeyNotFoundException(
                    $"Reservation hold '{holdId}' was not found.");
            }

            if (hold.Status != ReservationHoldStatus.Active)
            {
                return;
            }

            hold.Status = ReservationHoldStatus.Released;

            await _publishEndpoint.Publish(
                new ReservationHoldReleasedIntegrationEvent(
                    hold.Id,
                    hold.UserId,
                    hold.ReservationId,
                    hold.ParkingSlotId,
                    hold.Reservation.ParkingSlotNumber,
                    DateTime.UtcNow,
                    "Hold released by user"
                ),
                cancellationToken
            );

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<ReservationDetailsDto> UpdateReservationAsync(
            Guid reservationId, string userId, UpdateReservationRequest request, 
            CancellationToken cancellationToken = default)
        {
            var reservation =
            await GetOwnedReservationAsync(reservationId, userId, cancellationToken);

            EnsureModifiable(reservation);
            ValidateReservationDates(request.StartTimeUtc, request.EndTimeUtc);

            var available = await IsSlotAvailableAsync(
                reservation.ParkingSlotId,
                request.StartTimeUtc,
                request.EndTimeUtc,
                reservation.Id,
                cancellationToken);

            if (!available)
            {
                throw new InvalidOperationException(
                    "The parking slot is no longer available for the requested period.");
            }

            reservation.StartTimeUtc = request.StartTimeUtc;
            reservation.EndTimeUtc = request.EndTimeUtc;
            reservation.ReservationType = request.ReservationType;
            reservation.AccessMethod = request.AccessMethod;
            reservation.UpdatedAtUtc = DateTime.UtcNow;
            reservation.Amount = CalculateAmount(reservation.StartTimeUtc,
                reservation.EndTimeUtc, reservation.ReservationType);

            await _publishEndpoint.Publish(
                new ReservationUpdatedIntegrationEvent(
                    reservation.Id,
                    reservation.ReservationNumber,
                    reservation.ParkingLotId,
                    reservation.ParkingZoneId,
                    reservation.ParkingSlotId,
                    reservation.StartTimeUtc,
                    reservation.EndTimeUtc,
                    reservation.Amount,
                    reservation.CurrencyCode),
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return MapToDetailsDto(reservation);
        }

        // Additional private helper methods would go here, such as:
        private IQueryable<Models.Reservation> BuildSearchQuery(ReservationSearchRequest request)
        {
            var query = _context.Reservations.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim();

                query = query.Where(x =>
                    x.ReservationNumber.Contains(search) ||
                    x.ParkingLotName.Contains(search) ||
                    x.ParkingSlotNumber.Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(request.UserId))
            {
                query = query.Where(x => x.UserId == request.UserId);
            }

            if (request.ParkingLotId.HasValue)
            {
                query = query.Where(x => 
                x.ParkingLotId == request.ParkingLotId.Value);
            }

            if (request.ParkingZoneId.HasValue)
            {
                query = query.Where(x => 
                    x.ParkingZoneId == request.ParkingZoneId.Value);
            }

            if (request.ParkingSlotId.HasValue)
            {
                query = query.Where(x =>
                    x.ParkingSlotId == request.ParkingSlotId.Value);
            }

            if (request.VehicleId.HasValue)
            {
                query = query.Where(x =>
                    x.VehicleId == request.VehicleId.Value);
            }

            if (request.Status.HasValue)
            {
                query = query.Where(x =>
                    x.Status == request.Status.Value);
            }

            if (request.PaymentStatus.HasValue)
            {
                query = query.Where(x =>
                    x.PaymentStatus == request.PaymentStatus.Value);
            }

            if (request.ReservationType.HasValue)
            {
                query = query.Where(x =>
                    x.ReservationType == request.ReservationType.Value);
            }

            if (request.StartDateUtc.HasValue)
            {
                query = query.Where(x =>
                    x.StartTimeUtc >= request.StartDateUtc.Value);
            }

            if (request.EndDateUtc.HasValue)
            {
                query = query.Where(x =>
                    x.EndTimeUtc <= request.EndDateUtc.Value);
            }

            return query;
        }

        private async Task<Models.Reservation> GetReservationAsync(
            Guid reservationId, CancellationToken cancellationToken)
        {
            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(x => x.Id == reservationId, cancellationToken);

            if (reservation == null)
            {
                throw new KeyNotFoundException(
                    $"Reservation '{reservationId}' was not found.");
            }

            return reservation;
        }

        private async Task<Models.Reservation> GetOwnedReservationAsync(
            Guid reservationId, string userId, CancellationToken cancellationToken)
        {
            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(x => x.Id == reservationId &&
                    x.UserId == userId, cancellationToken);

            if (reservation == null)
            {
                throw new KeyNotFoundException(
                    $"Reservation '{reservationId}' was not found.");
            }

            return reservation;
        }

        private async Task<bool> IsSlotAvailableAsync(Guid parkingSlotId,
            DateTime startTimeUtc, DateTime endTimeUtc, Guid? excludeReservationId,
            CancellationToken cancellationToken)
        {
            var query = _context.Reservations.Where(x =>
                x.ParkingSlotId == parkingSlotId &&
                x.StartTimeUtc < endTimeUtc &&
                x.EndTimeUtc > startTimeUtc &&
                x.Status != ReservationStatus.Cancelled &&
                x.Status != ReservationStatus.Expired &&
                x.Status != ReservationStatus.NoShow);

            if (excludeReservationId.HasValue)
            {
                query = query.Where(x => x.Id != excludeReservationId.Value);
            }

            return !await query.AnyAsync(cancellationToken);
        }

        private static void ValidateReservationDates(DateTime startTimeUtc, DateTime endTimeUtc)
        {
            if (startTimeUtc >= endTimeUtc)
            {
                throw new ArgumentException(
                    "Reservation start time must be before end time.");
            }

            if (startTimeUtc.Kind != DateTimeKind.Utc || endTimeUtc.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException(
                    "Reservation times must be specified in UTC.");
            }
        }

        private static void EnsureModifiable(Models.Reservation reservation)
        {
            if (reservation.Status is ReservationStatus.Active or
                ReservationStatus.Completed or ReservationStatus.Cancelled or
                ReservationStatus.Expired or ReservationStatus.NoShow)
            {
                throw new InvalidOperationException(
                    $"Reservation cannot be modified while in {reservation.Status} status.");
            }
        }

        private static decimal CalculateAmount(CreateReservationRequest request)
        {
            return CalculateAmount(request.StartTimeUtc, request.EndTimeUtc, request.ReservationType);
        }

        private static decimal CalculateAmount(DateTime start, DateTime end, ReservationType type)
        {
            var hours = (decimal)(end - start).TotalHours;

            return type switch
            {
                ReservationType.Hourly => Math.Ceiling(hours),
                ReservationType.Daily => Convert.ToDecimal(Math.Ceiling((end - start).TotalDays)),
                ReservationType.Monthly => 1,
                ReservationType.Subscription =>1,
                _ => 0
            };
        }

        private static string GenerateReservationNumber(DateTime utcNow)
        {
            return
                $"PL-{utcNow:yyyyMMdd}-" +
                $"{Guid.NewGuid():N}"[..6]
                    .ToUpperInvariant();
        }

        private static ReservationListItemDto MapToListItemDto(Models.Reservation reservation)
        {
            return new ReservationListItemDto
            {
                Id = reservation.Id,
                ReservationNumber = reservation.ReservationNumber,
                VehicleId = reservation.VehicleId,
                ParkingLotName = reservation.ParkingLotName,
                ParkingSlotNumber = reservation.ParkingSlotNumber,
                Status = reservation.Status,
                PaymentStatus = reservation.PaymentStatus,
                StartTimeUtc = reservation.StartTimeUtc,
                EndTimeUtc = reservation.EndTimeUtc,
                Amount = reservation.Amount,
                CurrencyCode = reservation.CurrencyCode
            };
        }

        private static ReservationDetailsDto MapToDetailsDto(Models.Reservation reservation)
        {
            return new ReservationDetailsDto
            {
                Id = reservation.Id,
                ReservationNumber = reservation.ReservationNumber,
                UserId = reservation.UserId,
                VehicleId = reservation.VehicleId,
                ParkingLotId = reservation.ParkingLotId,
                ParkingZoneId = reservation.ParkingZoneId,
                ParkingSlotId = reservation.ParkingSlotId,
                ParkingLotName = reservation.ParkingLotName,
                ParkingSlotNumber = reservation.ParkingSlotNumber,
                ReservationType = reservation.ReservationType,
                Status = reservation.Status,
                PaymentStatus = reservation.PaymentStatus,
                AccessMethod = reservation.AccessMethod,
                StartTimeUtc = reservation.StartTimeUtc,
                EndTimeUtc = reservation.EndTimeUtc,
                ActualEntryTimeUtc = reservation.ActualEntryTimeUtc,
                ActualExitTimeUtc = reservation.ActualExitTimeUtc,
                Amount = reservation.Amount,
                CurrencyCode = reservation.CurrencyCode,
                PaymentReference = reservation.PaymentReference,
                AccessCredential = reservation.AccessCredential,
                CreatedAtUtc = reservation.CreatedAtUtc,
                UpdatedAtUtc = reservation.UpdatedAtUtc
            };
        }
    }
}
