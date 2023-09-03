using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.Text;

namespace OAuth2Simplified.Pages.Google
{
    public class IndexModel : PageModel
    {
        private readonly GoogleClientSettings _googleClientSettings;

        public IndexModel(IOptions<GoogleClientSettings> options)
        {
            _googleClientSettings = options.Value;
        }
        public void OnGet()
        {
        }

        public IActionResult OnGetLogin()
        {
            AppSession.State = AppSession.GenerateState();

            var scope = "openid email profile";
            var postData = new Dictionary<string, string>
            {

                { "response_type", "code" },
                { "client_id", _googleClientSettings.ClientId },
                { "redirect_uri", _googleClientSettings.RedirectUrl },
                { "scope", scope },
                { "state", AppSession.State }
            };

            // Redirect the user to Google's authorization page
            string redirectUrl = $"{_googleClientSettings.AuthorizeUrl}?{ToQueryString(postData)}";

            if (AppSession.GoogleAccessToken != null)
            {
                return RedirectToPage("google/data");
            }
            return Redirect(redirectUrl);
        }

        private static string ToQueryString(Dictionary<string, string> parameters)
        {
            var queryString = new StringBuilder();
            foreach (var param in parameters)
            {
                if (queryString.Length > 0)
                {
                    queryString.Append('&');
                }
                queryString.Append($"{param.Key}={Uri.EscapeDataString(param.Value)}");
            }
            return queryString.ToString();
        }
    }
}