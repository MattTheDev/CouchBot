using Newtonsoft.Json;
using System.Collections.Generic;

namespace MTD.CouchBot.Domain.Models.Twitch
{
    public class TwitchGameResponse
    {
        [JsonProperty("_total")]
        public int Tota { get; set; }
        [JsonProperty("streams")]
        public List<Stream> Streams { get; set; }

        public class Preview
        {
            [JsonProperty("small")]
            public string Small { get; set; }
            [JsonProperty("medium")]
            public string Medium { get; set; }
            [JsonProperty("large")]
            public string Large { get; set; }
            [JsonProperty("template")]
            public string Template { get; set; }
        }

        public class Channel
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
            public int Id { get; set; }
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
            public string ProfileBanner { get; set; }
            [JsonProperty("profile_banner_background_color")]
            public string ProfileBackgroundColor { get; set; }
            [JsonProperty("url")]
            public string Url { get; set; }
            [JsonProperty("views")]
            public int Views { get; set; }
            [JsonProperty("followers")]
            public int Followers { get; set; }
            [JsonProperty("broadcaster_type")]
            public string BroadcasterType { get; set; }
            [JsonProperty("description")]
            public string Description { get; set; }
        }

        public class Stream
        {
            [JsonProperty("_id")]
            public object Id { get; set; }
            [JsonProperty("game")]
            public string Game { get; set; }
            [JsonProperty("broadcast_platform")]
            public string BroadcastPlatform { get; set; }
            [JsonProperty("community_id")]
            public string CommunityId { get; set; }
            [JsonProperty("viewers")]
            public int Viewers { get; set; }
            [JsonProperty("video_height")]
            public int VideoHeight { get; set; }
            [JsonProperty("average_fps")]
            public double AverageFps { get; set; }
            [JsonProperty("delay")]
            public int Delay { get; set; }
            [JsonProperty("created_at")]
            public string CreatedAt { get; set; }
            [JsonProperty("is_playlist")]
            public bool IsPlaylist { get; set; }
            [JsonProperty("stream_type")]
            public string StreamType { get; set; }
            [JsonProperty("preview")]
            public Preview Preview { get; set; }
            [JsonProperty("channel")]
            public Channel Channel { get; set; }
        }
    }
}
