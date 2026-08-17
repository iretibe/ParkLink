using ParkLink.Payment.Dtos;
using ParkLink.SharedKernel.Pagination;

namespace ParkLink.Payment.Services
{
    public interface IPaymentService
    {
        Task<PaymentDto> CreatePaymentAsync(
            string userId, CreatePaymentRequest request,
            CancellationToken cancellationToken = default);
        Task<PaymentDto?> GetPaymentByIdAsync(Guid paymentId,
            string UserId, CancellationToken cancellationToken = default);
        Task<PaymentDto?> GetPaymentByReservationIdAsync(
            Guid reservationId, string userId, CancellationToken cancellationToken = default);
        Task<PagedResult<PaymentDto>> GetPaymentsAsync(
            PaymentSearchRequest request,
            CancellationToken cancellationToken = default);
        Task<PaymentDto> VerifyPaymentAsync(Guid paymentId,
            string userId, CancellationToken cancellationToken = default);
        Task<PaymentDto> RefundPaymentAsync(
            Guid paymentId, RefundPaymentRequest request,
            CancellationToken cancellationToken = default);
        Task<PaymentStatisticsDto> GetStatisticsAsync(
            CancellationToken cancellationToken = default);
        Task ProcessPaystackWebhookAsync(string payload,
            string signature, CancellationToken cancellationToken = default);
    }
}
