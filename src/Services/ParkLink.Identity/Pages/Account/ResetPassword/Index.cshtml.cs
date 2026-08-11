using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using ParkLink.Identity.Models;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ParkLink.Identity.Pages.Account.ResetPassword
{
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            ILogger<IndexModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public bool ResetSuccessful { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required]
            public string Token { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            [StringLength(
                100,
                MinimumLength = 8,
                ErrorMessage = "Password must be at least 8 characters long.")]
            public string Password { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            [Compare(
                nameof(Password),
                ErrorMessage = "The passwords do not match.")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        public IActionResult OnGet(
            string? email,
            string? token)
        {
            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(token))
            {
                return RedirectToPage(
                    "/Account/ForgotPassword/Index");
            }

            try
            {
                Input.Email = email;

                Input.Token =
                    Encoding.UTF8.GetString(
                        WebEncoders.Base64UrlDecode(token));

                return Page();
            }
            catch (FormatException)
            {
                _logger.LogWarning(
                    "Invalid password reset token received for email {Email}",
                    email);

                return RedirectToPage(
                    "/Account/ForgotPassword/Index");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user =
                await _userManager.FindByEmailAsync(
                    Input.Email);

            // Do not reveal whether the email exists.
            if (user == null)
            {
                return RedirectToPage(
                    "/Account/ResetPassword/Confirmation");
            }

            var result =
                await _userManager.ResetPasswordAsync(
                    user,
                    Input.Token,
                    Input.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation(
                    "Password reset successfully for user {UserId}",
                    user.Id);

                return RedirectToPage(
                    "/Account/ResetPassword/Confirmation");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(
                    string.Empty,
                    error.Description);
            }

            return Page();
        }
    }
}