using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ParkLink.Identity.Pages.Account.ResetPassword
{
    [AllowAnonymous]
    public class ConfirmationModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}