using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParkLink.Payment.Services;

namespace ParkLink.Payment.Controllers
{
    public sealed class PaystackWebhookController : BaseController
    {
        private readonly IPaymentService _paymentService;
        private readonly IPaystackWebhookValidator _validator;
        private readonly ILogger<PaystackWebhookController> _logger;

        public PaystackWebhookController(
            IPaymentService paymentService,
            IPaystackWebhookValidator validator,
            ILogger<PaystackWebhookController> logger)
        {
            _paymentService = paymentService;
            _validator = validator;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("paystack")]
        public async Task<IActionResult> Paystack(CancellationToken cancellationToken)
        {
            Request.EnableBuffering();

            using var reader = new StreamReader(Request.Body);

            var payload = await reader.ReadToEndAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(payload))
            {
                return BadRequest();
            }

            var signature = Request.Headers["x-paystack-signature"].FirstOrDefault();

            if (!_validator.Validate(payload, signature ?? string.Empty))
            {
                _logger.LogWarning("Invalid Paystack webhook signature.");

                return Unauthorized();
            }

            try
            {
                await _paymentService.ProcessPaystackWebhookAsync(payload, signature ?? string.Empty, cancellationToken);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Paystack webhook.");

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
