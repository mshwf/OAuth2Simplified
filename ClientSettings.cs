namespace OAuth2Simplified
{
    public class ClientSettings
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string ApiUrl { get; set; }
        public string AuthorizeUrl { get; set; }
        public string AccessTokenUrl { get; set; }
        public string RedirectUrl { get; set; }
    }

    public class GitHubClientSettings : ClientSettings { }

    public class GoogleClientSettings : ClientSettings { }
}