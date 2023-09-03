using System.Text.Json;

namespace OAuth2Simplified
{
    public class Json
    {
        public static T Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        });
    }
}