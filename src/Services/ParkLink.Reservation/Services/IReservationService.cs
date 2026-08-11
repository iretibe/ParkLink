using ParkLink.Reservation.Dtos;
using ParkLink.SharedKernel.Pagination;

namespace ParkLink.Reservation.Services
{
    public interface IReservationService
    {
        Task<PagedResult<ReservationListItemDto>> GetReservationsAsync(
            ReservationSearchRequest request,
            CancellationToken cancellationToken = default);
        Task<ReservationDetailsDto?> GetReservationByIdAsync(
            Guid reservationId,
            CancellationToken cancellationToken = default);
        Task<ReservationDetailsDto?> GetMyReservationAsync(
            Guid reservationId, string userId,
            CancellationToken cancellationToken = default);
        Task<PagedResult<ReservationListItemDto>> GetMyReservationsAsync(
            string userId, ReservationSearchRequest request,
            CancellationToken cancellationToken = default);
        Task<ReservationDetailsDto> CreateReservationAsync(string userId,
            CreateReservationRequest request,
            CancellationToken cancellationToken = default);
        Task<ReservationDetailsDto> UpdateReservationAsync(Guid reservationId, 
            string userId, UpdateReservationRequest request,
            CancellationToken cancellationToken = default);
        Task<ReservationDetailsDto> ExtendReservationAsync(Guid reservationId, 
            string userId, ExtendReservationRequest request,
            CancellationToken cancellationToken = default);
        Task CancelReservationAsync(Guid reservationId,
            string userId, CancelReservationRequest request,
            CancellationToken cancellationToken = default);
        Task<ReservationDetailsDto> ConfirmReservationAsync(Guid reservationId,
            CancellationToken cancellationToken = default);
        Task<ReservationDetailsDto> ActivateReservationAsync(Guid reservationId,
            CancellationToken cancellationToken = default);
        Task<ReservationDetailsDto> CompleteReservationAsync(
            Guid reservationId, CancellationToken cancellationToken = default);
        Task ExpireReservationAsync(Guid reservationId,
            CancellationToken cancellationToken = default);
        Task MarkReservationAsNoShowAsync(Guid reservationId,
            CancellationToken cancellationToken = default);
        Task<ReservationHoldDto> CreateHoldAsync(string userId,
            CreateReservationHoldRequest request,
            CancellationToken cancellationToken = default);
        Task ReleaseHoldAsync(Guid holdId, string userId,
            CancellationToken cancellationToken = default);
        Task<ReservationAvailabilityDto> CheckAvailabilityAsync(
            ReservationAvailabilityRequest request,
            CancellationToken cancellationToken = default);
        Task<ReservationDetailsDto> CheckInAsync(
            Guid reservationId, CheckInRequest request,
            CancellationToken cancellationToken = default);
        Task<ReservationDetailsDto> CheckOutAsync(
            Guid reservationId, CheckOutRequest request,
            CancellationToken cancellationToken = default);
        Task<ReservationStatisticsDto> GetStatisticsAsync(
            CancellationToken cancellationToken = default);
        //Task StartReservationAsync(Guid id,
        //    CancellationToken cancellationToken = default);
    }
}
