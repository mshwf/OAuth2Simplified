using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.Text.Json.Nodes;

namespace OAuth2Simplified.Pages.Google
{
    public class CallbackModel : PageModel
    {
        private readonly GoogleClientSettings _googleClientSettings;

        public CallbackModel(IOptions<GoogleClientSettings> options)
        {
            _googleClientSettings = options.Value;
        }

        public async Task<IActionResult> OnGet(string code, string state)
        {
            if (AppSession.State != state)
            {
                throw new InvalidOperationException("State has changed, authorization broken!");
            }
            if (!string.IsNullOrWhiteSpace(code))
            {
                string tokenResponse = await RequestToken(code);
                var tokenNode = JsonNode.Parse(tokenResponse);
                var accessToken = tokenNode["access_token"];
                var idToken = tokenNode["id_token"];
                AppSession.GoogleAccessToken = accessToken?.ToString() ?? throw new InvalidOperationException("Couldn't exchange token!");
                AppSession.GoogleIdToken = idToken?.ToString() ?? throw new InvalidOperationException("Couldn't exchange id token!");

                // One way of getting the user info
                var userInfo = idToken.ToString().Split('.')[1];
                byte[] data = Convert.FromBase64String(userInfo);
                string decodedString = System.Text.Encoding.UTF8.GetString(data);
                var userInfoNode = JsonNode.Parse(decodedString);
                var googleUserId = userInfoNode["sub"].ToString();
                var googleUserEmail = userInfoNode["email"].ToString();
                return Redirect("/google");
            }
            return Redirect("Error");
        }

        private async Task<string> RequestToken(string code)
        {
            using HttpClient client = new();
            var postData = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "client_id", _googleClientSettings.ClientId },
                { "client_secret", _googleClientSettings.ClientSecret },
                { "code", code },
                { "redirect_uri", _googleClientSettings.RedirectUrl },
            };

            var content = new FormUrlEncodedContent(postData);
            var httpResponse = await client.PostAsync(_googleClientSettings.AccessTokenUrl, content);

            var result = await httpResponse.Content.ReadAsStringAsync();
            return result;
        }
    }
}