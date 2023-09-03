using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OAuth2Simplified.Pages
{
    public class IndexModel : PageModel
    {
        public IActionResult OnGet(bool logout)
        {
            if (logout)
            {
                AppSession.ClearSession();
                return Redirect("/");
            }
            return Page();
        }
    }
}