using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.Text.Json.Nodes;

namespace OAuth2Simplified.Pages.GitHub
{
    public class CallbackModel : PageModel
    {
        private readonly GitHubClientSettings _gitHubClientSettings;

        public CallbackModel(IOptions<GitHubClientSettings> options)
        {
            _gitHubClientSettings = options.Value;
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
                var node = JsonNode.Parse(tokenResponse);
                var token = node["access_token"];

                AppSession.GitHubAccessToken = token?.ToString() ?? throw new InvalidOperationException("Couldn't exchange token!");
                return Redirect("/github");
            }
            return Redirect("Error");
        }

        private async Task<string> RequestToken(string code)
        {
            using HttpClient client = new();
            var postData = new Dictionary<string, string>
            {
                { "client_id", _gitHubClientSettings.ClientId },
                { "client_secret", _gitHubClientSettings.ClientSecret },
                { "code", code },
                { "redirect_uri", _gitHubClientSettings.RedirectUrl },
            };
            var headers = new Dictionary<string, string>
            {
                { "Accept", "application/vnd.github.v3+json, application/json" },
                { "User-Agent", "mshwf" }
            };

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }

            HttpResponseMessage httpResponse;

            var content = new FormUrlEncodedContent(postData);
            httpResponse = await client.PostAsync(_gitHubClientSettings.AccessTokenUrl, content);

            var result = await httpResponse.Content.ReadAsStringAsync();
            return result;
        }
    }
}
