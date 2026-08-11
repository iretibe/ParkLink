using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ParkLink.Identity.Models;
using ParkLink.Identity.Services.Emails;
using System.ComponentModel.DataAnnotations;

namespace ParkLink.Identity.Pages.Account.ForgotPassword
{
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(UserManager<ApplicationUser> userManager,
            IEmailService emailService, ILogger<IndexModel> logger)
        {
            _userManager = userManager;
            _emailService = emailService;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public bool EmailSent { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user =
                await _userManager.FindByEmailAsync(Input.Email);

            // Always show the same result regardless of whether
            // the email exists. This prevents account enumeration.
            if (user == null)
            {
                _logger.LogInformation(
                    "Password reset requested for an unknown email.");

                EmailSent = true;
                return Page();
            }

            // Important: External-only accounts may not have a password.
            var hasPassword =
                await _userManager.HasPasswordAsync(user);

            if (!hasPassword)
            {
                EmailSent = true;
                return Page();
            }

            var token =
                await _userManager.GeneratePasswordResetTokenAsync(user);

            var encodedToken =
                Microsoft.AspNetCore.WebUtilities.WebEncoders
                    .Base64UrlEncode(
                        System.Text.Encoding.UTF8.GetBytes(token));

            var callbackUrl =
                Url.Page(
                    "/Account/ResetPassword/Index",
                    null,
                    new
                    {
                        token = encodedToken,
                        email = user.Email
                    },
                    Request.Scheme);

            if (callbackUrl == null)
            {
                throw new InvalidOperationException(
                    "Unable to generate password reset URL.");
            }

            await _emailService.SendPasswordResetEmailAsync(
                user.Email!,
                user.FirstName,
                callbackUrl);

            EmailSent = true;

            return Page();
        }
    }
}
