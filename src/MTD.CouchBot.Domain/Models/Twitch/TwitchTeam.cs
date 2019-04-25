using Newtonsoft.Json;
using System.Collections.Generic;

namespace MTD.CouchBot.Domain.Models.Twitch
{
    public class TwitchTeam
    {
        [JsonProperty("_id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("info")]
        public string Info { get; set; }
        [JsonProperty("display_name")]
        public string DisplayName { get; set; }
        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }
        [JsonProperty("updated_at")]
        public string UpdatedAt { get; set; }
        [JsonProperty("logo")]
        public string Logo { get; set; }
        [JsonProperty("banner")]
        public object Banner { get; set; }
        [JsonProperty("background")]
        public string Background { get; set; }
        [JsonProperty("users")]
        public List<User> Users { get; set; }

        public class User
        {
            [JsonProperty("mature")]
            public bool? Mature { get; set; }
            [JsonProperty("status")]
            public string Status { get; set; }
            [JsonProperty("broadcaster_language")]
            public string BroadcasterLanguage { get; set; }
            [JsonProperty("display_name")]
            public string DisplayName { get; set; }
            [JsonProperty("game")]
            public string Game { get; set; }
            [JsonProperty("language")]
            public string Language { get; set; }
            [JsonProperty("name")]
            public string Name { get; set; }
            [JsonProperty("created_at")]
            public string CreatedAt { get; set; }
            [JsonProperty("updated_at")]
            public string UpdatedAt { get; set; }
            [JsonProperty("_id")]
            public string Id { get; set; }
            [JsonProperty("logo")]
            public string Logo { get; set; }
            [JsonProperty("video_banner")]
            public string VideoBanner { get; set; }
            [JsonProperty("profile_banner")]
            public string ProfileBanner { get; set; }
            [JsonProperty("profile_banner_background_color")]
            public string ProfileBannerBackgroundColor { get; set; }
            [JsonProperty("partner")]
            public bool? Partner { get; set; }
            [JsonProperty("url")]
            public string Url { get; set; }
            [JsonProperty("views")]
            public int Views { get; set; }
            [JsonProperty("followers")]
            public int Followers { get; set; }
        }
    }
}
