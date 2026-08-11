using Duende.IdentityModel;
using Duende.IdentityServer;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ParkLink.Identity.Models;

namespace ParkLink.Identity.Pages.Logout
{
    [SecurityHeaders]
    [AllowAnonymous]
    public class Index : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IEventService _events;

        [BindProperty]
        public string? LogoutId { get; set; }

        public Index(SignInManager<ApplicationUser> signInManager, IIdentityServerInteractionService interaction, IEventService events)
        {
            _signInManager = signInManager;
            _interaction = interaction;
            _events = events;
        }

        public async Task<IActionResult> OnGet(string? logoutId)
        {
            LogoutId = logoutId;

            var showLogoutPrompt = LogoutOptions.ShowLogoutPrompt;

            if (User.Identity?.IsAuthenticated != true)
            {
                // if the user is not authenticated, then just show logged out page
                showLogoutPrompt = false;
            }
            else
            {
                var context = await _interaction.GetLogoutContextAsync(LogoutId);
                if (context?.ShowSignoutPrompt == false)
                {
                    // it's safe to automatically sign-out
                    showLogoutPrompt = false;
                }
            }

            if (showLogoutPrompt == false)
            {
                // if the request for logout was properly authenticated from IdentityServer, then
                // we don't need to show the prompt and can just log the user out directly.
                return await OnPost();
            }

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            LogoutId ??= await _interaction.CreateLogoutContextAsync();

            var context = await _interaction.GetLogoutContextAsync(LogoutId);

            if (User.Identity?.IsAuthenticated == true)
            {
                var idp = User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;

                // Sign out local cookie
                await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

                await _events.RaiseAsync(new UserLogoutSuccessEvent(
                    User.GetSubjectId(), User.GetDisplayName()));

                // External IdP logout (if any)
                if (!string.IsNullOrEmpty(idp) &&
                    idp != IdentityServerConstants.LocalIdentityProvider)
                {
                    if (await HttpContext.GetSchemeSupportsSignOutAsync(idp))
                    {
                        return SignOut(
                            new AuthenticationProperties
                            {
                                RedirectUri = context?.PostLogoutRedirectUri ?? "~/"
                            },
                            idp);
                    }
                }
            }

            // Return to client (Blazor)
            if (!string.IsNullOrEmpty(context?.PostLogoutRedirectUri))
            {
                return Redirect(context.PostLogoutRedirectUri);
            }

            return Redirect("~/");
        }

        //public async Task<IActionResult> OnPost()
        //{
        //    if (User.Identity?.IsAuthenticated == true)
        //    {
        //        LogoutId ??= await _interaction.CreateLogoutContextAsync();

        //        // delete local authentication cookie
        //        await _signInManager.SignOutAsync();

        //        // see if we need to trigger federated logout
        //        var idp = User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;

        //        // raise the logout event
        //        await _events.RaiseAsync(new UserLogoutSuccessEvent(User.GetSubjectId(), User.GetDisplayName()));
        //        Telemetry.Metrics.UserLogout(idp);

        //        // if it's a local login we can ignore this workflow
        //        if (idp != null && idp != Duende.IdentityServer.IdentityServerConstants.LocalIdentityProvider)
        //        {
        //            // we need to see if the provider supports external logout
        //            if (await HttpContext.GetSchemeSupportsSignOutAsync(idp))
        //            {
        //                // build a return URL so the upstream provider will redirect back
        //                // to us after the user has logged out. this allows us to then
        //                // complete our single sign-out processing.
        //                var url = Url.Page("/Account/Logout/Loggedout", new { logoutId = LogoutId });

        //                // this triggers a redirect to the external provider for sign-out
        //                return SignOut(new AuthenticationProperties { RedirectUri = url }, idp);
        //            }
        //        }
        //    }

        //    return RedirectToPage("/Account/Logout/LoggedOut", new { logoutId = LogoutId });
        //}
    }
}
