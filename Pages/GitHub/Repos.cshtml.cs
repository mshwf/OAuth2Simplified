using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace OAuth2Simplified.Pages.GitHub
{
    public class Repo
    {
        public string Name { get; set; }
        public string Html_url { get; set; }
    }

    public class ReposModel : PageModel
    {
        private readonly GitHubClientSettings _githubClientSettings;

        public ReposModel(IOptions<GitHubClientSettings> options)
        {
            _githubClientSettings = options.Value;
        }
        public List<Repo> Repos { get; set; }

        public async Task<IActionResult> OnGet()
        {
            if (AppSession.GitHubAccessToken == null)
                return Redirect("/github");

            var result = await GetUserRepositories();
            Repos = Json.Deserialize<List<Repo>>(result);
            return Page();
        }

        async Task<string> GetUserRepositories()
        {
            using HttpClient client = new();
            var headers = new Dictionary<string, string>
            {
                { "Accept", "application/vnd.github.v3+json, application/json" },
                { "User-Agent", "mshwf" },
                { "Authorization", "Bearer " + AppSession.GitHubAccessToken }
            };
            foreach (var header in headers)
            {
                client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }

            var httpResponse = await client.GetAsync(_githubClientSettings.ApiUrl + "/user/repos?sort=created&direction=desc");
            var result = await httpResponse.Content.ReadAsStringAsync();
            return result;
        }
    }
}