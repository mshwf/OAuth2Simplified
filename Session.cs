using System.Security.Cryptography;

namespace OAuth2Simplified
{
    public class AppSession
    {
        public static string GitHubAccessToken { get; set; }
        public static string GoogleAccessToken { get; set; }
        public static string State { get; set; }
        public static string GoogleIdToken { get; set; }
        public static string GenerateState() => BitConverter.ToString(RandomNumberGenerator.GetBytes(16)).Replace("-", "").ToLower();
        public static void ClearSession()
        {
            GitHubAccessToken = null;
            GoogleAccessToken = null;
            GoogleIdToken = null;
            State = null;
        }
    }
}