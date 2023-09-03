using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace OAuth2Simplified.Pages.Google
{
    public class UserInfo
    {
        public string Email { get; set; }
        public string Name { get; set; }
    }

    public class UserInfoModel : PageModel
    {
        private readonly GoogleClientSettings _googleClientSettings;

        public UserInfoModel(IOptions<GoogleClientSettings> options)
        {
            _googleClientSettings = options.Value;
        }
        public UserInfo UserInfo { get; set; }
        public async Task<IActionResult> OnGet()
        {
            if (AppSession.GoogleAccessToken == null)
                return Redirect("/google");

            var userInfoResponse = await RequestUserInfo();
            UserInfo = Json.Deserialize<UserInfo>(userInfoResponse);
            return Page();
        }

        private async Task<string> RequestUserInfo()
        {
            using HttpClient client = new();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {AppSession.GoogleAccessToken}");
            var httpResponse = await client.GetAsync(_googleClientSettings.ApiUrl + "/v1/userinfo");
            var result = await httpResponse.Content.ReadAsStringAsync();
            return result;
        }
    }
}