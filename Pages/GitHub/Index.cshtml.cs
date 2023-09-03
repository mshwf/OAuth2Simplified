using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.Text;

namespace OAuth2Simplified.Pages.GitHub;

public class IndexModel : PageModel
{
    private readonly GitHubClientSettings _githubClientSettings;

    public IndexModel(IOptions<GitHubClientSettings> options)
    {
        _githubClientSettings = options.Value;
    }

    public void OnGet()
    {

    }

    public IActionResult OnGetLogin()
    {
        AppSession.State = AppSession.GenerateState();

        var scope = "public_repo";
        var postData = new Dictionary<string, string>
        {
            { "response_type", "code" },
            { "client_id", _githubClientSettings.ClientId },
            { "redirect_uri", _githubClientSettings.RedirectUrl },
            { "scope", scope },
            { "state", AppSession.State }
        };

        // Redirect the user to GitHub's authorization page
        string redirectUrl = $"{_githubClientSettings.AuthorizeUrl}?{ToQueryString(postData)}";

        if (AppSession.GitHubAccessToken != null)
        {
            return RedirectToPage("/github/repos");
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