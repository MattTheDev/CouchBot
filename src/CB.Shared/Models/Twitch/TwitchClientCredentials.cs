using Newtonsoft.Json;

namespace CB.Shared.Models.Twitch;

public class TwitchClientCredentials
{
    [JsonProperty("access_token")]
    public string AccessToken { get; set; }

    [JsonProperty("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonProperty("token_type")]
    public string TokenType { get; set; }
}