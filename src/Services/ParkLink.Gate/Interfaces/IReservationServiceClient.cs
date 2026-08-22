using ParkLink.Gate.Dtos;

namespace ParkLink.Gate.Interfaces
{
    public interface IReservationServiceClient
    {
        Task<ReservationAccessResult?> GetActiveReservationAsync(
            Guid vehicleId, Guid gateId, DateTime atUtc,
            CancellationToken cancellationToken = default);
    }
}
