using Microsoft.AspNetCore.Mvc;
using ParkLink.Payment.Dtos;
using ParkLink.Payment.Services;
using ParkLink.SharedKernel.Pagination;
using System.Security.Claims;

namespace ParkLink.Payment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : BaseController
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost]
        public async Task<ActionResult<PaymentDto>> Create(
            CreatePaymentRequest request, CancellationToken cancellationToken)
        {
            var userId = GetUserId();

            var result = await _paymentService.CreatePaymentAsync(
                userId, request, cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { paymentId = result.Id },
                result
            );
        }

        [HttpGet("{paymentId:guid}")]
        public async Task<ActionResult<PaymentDto>> GetById(
            Guid paymentId, CancellationToken cancellationToken)
        {
            var result = await _paymentService.GetPaymentByIdAsync(
                paymentId, cancellationToken);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpGet("reservation/{reservationId:guid}")]
        public async Task<ActionResult<PaymentDto>> GetByReservation(
            Guid reservationId, CancellationToken cancellationToken)
        {
            var result = await _paymentService.GetPaymentByReservationIdAsync(
                reservationId, cancellationToken);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<PaymentDto>>> GetPayments(
            [FromQuery] PaymentSearchRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _paymentService.GetPaymentsAsync(request, cancellationToken);

            return Ok(result);
        }

        [HttpPost("{paymentId:guid}/verify")]
        public async Task<ActionResult<PaymentDto>> Verify(
            Guid paymentId, CancellationToken cancellationToken)
        {
            var result = await _paymentService.VerifyPaymentAsync(
                paymentId, cancellationToken);

            return Ok(result);
        }

        [HttpPost("{paymentId:guid}/refund")]
        public async Task<ActionResult<PaymentDto>> Refund(
            Guid paymentId, RefundPaymentRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _paymentService.RefundPaymentAsync(
                paymentId, request, cancellationToken);

            return Ok(result);
        }

        [HttpGet("statistics")]
        public async Task<ActionResult<PaymentStatisticsDto>> Statistics(
            CancellationToken cancellationToken)
        {
            var result = await _paymentService.GetStatisticsAsync(cancellationToken);

            return Ok(result);
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub")
                ?? throw new UnauthorizedAccessException(
                    "Authenticated user ID was not found.");
        }
    }
}
