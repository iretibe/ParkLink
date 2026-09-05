using Microsoft.EntityFrameworkCore;
using ParkLink.Gate.Data;
using ParkLink.Gate.Dtos;
using ParkLink.Gate.Entities;
using ParkLink.Gate.Enums;
using ParkLink.Gate.Interfaces;
using ParkLink.Gate.Services.Interfaces;

namespace ParkLink.Gate.Services.Implementations
{
    public sealed class GateAccessService : IGateAccessService
    {
        private readonly GateContext _context;
        private readonly IVehicleServiceClient _vehicleService;
        private readonly IReservationServiceClient _reservationService;
        private readonly IPaymentServiceClient _paymentService;
        private readonly IGateDeviceCommandService _commandService;

        public GateAccessService(GateContext context,
            IVehicleServiceClient vehicleService,
            IReservationServiceClient reservationService,
            IPaymentServiceClient paymentService,
            IGateDeviceCommandService commandService)
        {
            _context = context;
            _vehicleService = vehicleService;
            _reservationService = reservationService;
            _paymentService = paymentService;
            _commandService = commandService;
        }

        public async Task<AccessDecisionResult> ProcessAccessAsync(AccessRequest request, CancellationToken cancellationToken = default)
        {
            var gate = await _context.Gates
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == request.GateId,
                cancellationToken);

            if (gate is null)
                throw new KeyNotFoundException("Gate was not found.");

            var attempt = GateAccessAttempt.Create(
                request.GateId,
                request.DeviceId,
                request.Method,
                request.DetectedAtUtc,
                request.LicensePlate,
                request.RfidTagIdentifier);

            _context.GateAccessAttempts.Add(attempt);

            await _context.SaveChangesAsync(cancellationToken);

            attempt.StartValidation();

            await _context.SaveChangesAsync(cancellationToken);

            try
            {
                VehicleLookupResult? vehicle;

                if (request.Method == AccessMethod.Rfid)
                {
                    if (string.IsNullOrWhiteSpace(
                        request.RfidTagIdentifier))
                    {
                        return await DenyAsync(
                            attempt,
                            "RFID tag identifier is required.",
                            cancellationToken
                        );
                    }

                    vehicle = await _vehicleService
                        .FindByRfidAsync(
                            request.RfidTagIdentifier,
                            cancellationToken);
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(
                        request.LicensePlate))
                    {
                        return await DenyAsync(
                            attempt,
                            "License plate is required.",
                            cancellationToken
                        );
                    }

                    vehicle = await _vehicleService
                        .FindByLicensePlateAsync(request.LicensePlate, cancellationToken);
                }

                if (vehicle is null)
                {
                    return await DenyAsync(
                        attempt,
                        "Vehicle could not be identified.",
                        cancellationToken);
                }

                if (!vehicle.IsActive)
                {
                    return await DenyAsync(
                        attempt,
                        "Vehicle is inactive.",
                        cancellationToken);
                }

                attempt.SetIdentity(vehicle.VehicleId, vehicle.UserId, null);

                await _context.SaveChangesAsync(cancellationToken);

                var reservation = await _reservationService.GetActiveReservationAsync(
                    vehicle.VehicleId, request.GateId, request.DetectedAtUtc,
                    cancellationToken
                );

                if (reservation is null)
                {
                    return await DenyAsync(attempt,
                        "No active reservation was found.",
                        cancellationToken
                    );
                }

                attempt.SetIdentity(vehicle.VehicleId, 
                    vehicle.UserId, reservation.ReservationId);

                await _context.SaveChangesAsync(cancellationToken);

                if (!reservation.IsValid)
                {
                    return await DenyAsync(attempt,
                        "Reservation is not valid.", cancellationToken);
                }

                var payment = await _paymentService.GetPaymentForReservationAsync(
                    reservation.ReservationId, cancellationToken);

                if (payment is null)
                {
                    return await DenyAsync(
                        attempt,
                        "Payment information could not be found.",
                        cancellationToken
                    );
                }

                if (!payment.IsValid)
                {
                    return await DenyAsync(
                        attempt,
                        "Reservation payment is not valid.",
                        cancellationToken
                    );
                }

                attempt.Grant("Vehicle, reservation and payment validated.");

                await _context.SaveChangesAsync(cancellationToken);

                var gateCommand = await _commandService.OpenGateAsync(
                    request.GateId, attempt.Id, cancellationToken);

                if (!gateCommand.Success)
                {
                    attempt.Fail(gateCommand.ErrorMessage ?? "Failed to open gate.");

                    await _context.SaveChangesAsync(cancellationToken);

                    return BuildResult(attempt, vehicle, reservation);
                }

                attempt.MarkGateOpened();
                attempt.Complete();

                await _context.SaveChangesAsync(cancellationToken);

                return BuildResult(attempt, vehicle, reservation);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                attempt.Fail($"Access processing failed: {ex.Message}");

                attempt.Complete();

                await _context.SaveChangesAsync(cancellationToken);

                return new AccessDecisionResult(
                    attempt.Id,
                    AccessDecision.Error,
                    attempt.DecisionReason ?? "Access processing failed.",
                    attempt.VehicleId,
                    attempt.ReservationId,
                    attempt.LicensePlate,
                    attempt.RfidTagIdentifier,
                    false
                );
            }
        }

        private async Task<AccessDecisionResult> DenyAsync(
            GateAccessAttempt attempt, string reason,
            CancellationToken cancellationToken)
        {
            attempt.Deny(reason);
            attempt.Complete();

            await _context.SaveChangesAsync(cancellationToken);

            return new AccessDecisionResult(
                attempt.Id,
                AccessDecision.Denied,
                reason,
                attempt.VehicleId,
                attempt.ReservationId,
                attempt.LicensePlate,
                attempt.RfidTagIdentifier,
                false
            );
        }

        private static AccessDecisionResult BuildResult(
            GateAccessAttempt attempt, VehicleLookupResult vehicle,
            ReservationAccessResult reservation)
        {
            return new AccessDecisionResult(
                attempt.Id,
                attempt.Decision,
                attempt.DecisionReason ?? string.Empty,
                vehicle.VehicleId,
                reservation.ReservationId,
                vehicle.LicensePlate,
                attempt.RfidTagIdentifier,
                attempt.GateOpenedAtUtc.HasValue
            );
        }
    }
}
