using Newtonsoft.Json;

namespace MTD.CouchBot.Domain.Models.Twitch
{
    public class TwitchChannelResponse
    {
        [JsonProperty("mature")]
        public bool Mature { get; set; }
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
        [JsonProperty("_id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }
        [JsonProperty("updated_at")]
        public string UpdatedAt { get; set; }
        [JsonProperty("partner")]
        public bool Partner { get; set; }
        [JsonProperty("logo")]
        public string Logo { get; set; }
        [JsonProperty("video_banner")]
        public string VideoBanner { get; set; }
        [JsonProperty("profile_banner")]
        public object ProfileBanner { get; set; }
        [JsonProperty("profile_banner_background_color")]
        public object ProfileBannerBackgroundColor { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("views")]
        public int Views { get; set; }
        [JsonProperty("followers")]
        public int Followers { get; set; }
        [JsonProperty("broadcaster_type")]
        public string BroadcasterTyle { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
