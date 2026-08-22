using ParkLink.Gate.Dtos;

namespace ParkLink.Gate.Interfaces
{
    public interface IPaymentServiceClient
    {
        Task<PaymentAccessResult?> GetPaymentForReservationAsync(
            Guid reservationId,
            CancellationToken cancellationToken = default);
    }
}
