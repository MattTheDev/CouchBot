using Newtonsoft.Json;

namespace CB.Shared.Models.Twitch;

public class TwitchTeam
{
    public List<Datum> Data { get; set; }

    public class User
    {
        [JsonProperty("user_id")] public string UserId { get; set; }

        [JsonProperty("user_name")] public string UserName { get; set; }

        [JsonProperty("user_login")] public string UserLogin { get; set; }
    }

    public class Datum
    {
        public List<User> Users { get; set; }

        [JsonProperty("background_image_url")] public string BackgroundImageUrl { get; set; }

        public object Banner { get; set; }

        [JsonProperty("created_at")] public string CreatedAt { get; set; }

        [JsonProperty("updated_at")] public string UpdatedAt { get; set; }

        public string Info { get; set; }

        [JsonProperty("thumbnail_url")] public string ThumbnailUrl { get; set; }

        [JsonProperty("team_name")] public string TeamName { get; set; }

        [JsonProperty("team_display_name")] public string TeamDisplayName { get; set; }

        public string Id { get; set; }
    }
}