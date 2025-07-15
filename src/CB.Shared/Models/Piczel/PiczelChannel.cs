using Newtonsoft.Json;

namespace CB.Shared.Models.Piczel;

public class PiczelChannel
{
public string Type { get; set; }

    public List<Datum> Data { get; set; }

    [JsonProperty("meta")]
    public Meta Meta_ { get; set; }

    public class OfflineImage
    {
        public object Url { get; set; }
    }

    public class Banner2
    {
        public object Url { get; set; }
    }

    public class Banner
    {
        [JsonProperty("banner")]
        public Banner2 Banner_ { get; set; }
    }

    public class Preview
    {
        public object Url { get; set; }
    }

    public class Basic
    {
        public bool Listed { get; set; }

        public bool AllowAnon { get; set; }

        public bool Notifications { get; set; }
    }

    public class Recording
    {
        public bool Enabled { get; set; }

        public bool Download { get; set; }

        [JsonProperty("timelapse_speed")]
        public int TimelapseSpeed { get; set; }

        [JsonProperty("watermark_timelapse")]
        public bool WatermarkTimelapse { get; set; }

        [JsonProperty("gen_timelapse")]
        public bool GenTimelapse { get; set; }
    }

    public class Private
    {
        public bool Enabled { get; set; }

        public bool Moderated { get; set; }
    }

    public class Emails
    {
        public bool Enabled { get; set; }
    }

    public class Settings
    {
        public Basic Basic { get; set; }

        public Recording Recording { get; set; }

        public Private Private { get; set; }

        public Emails Emails { get; set; }
    }

    public class Avatar
    {
        public string Url { get; set; }
    }

    public class Gallery
    {
        [JsonProperty("profile_description")]
        public string ProfileDescription { get; set; }
    }

    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; }

        [JsonProperty("premium?")]
        public bool Premium { get; set; }

        public Avatar Avatar { get; set; }

        public string Role { get; set; }

        public Gallery Gallery { get; set; }

        [JsonProperty("follower_count")]
        public int FollowerCount { get; set; }
    }

    public class Datum
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public object Description { get; set; }

        [JsonProperty("rendered_description")]
        public string RenderedDescription { get; set; }

        [JsonProperty("follower_count")]
        public int FollowerCount { get; set; }

        public bool Live { get; set; }

        [JsonProperty("live_since")]
        public DateTime? LiveSince { get; set; }

        [JsonProperty("isPrivate?")]
        public bool IsPrivate { get; set; }

        public string Slug { get; set; }

        [JsonProperty("offline_image")]
        public OfflineImage OfflineImage { get; set; }

        public Banner Banner { get; set; }

        [JsonProperty("banner_link")]
        public object BannerLink { get; set; }

        public Preview Preview { get; set; }

        public bool Adult { get; set; }

        [JsonProperty("in_multi")]
        public bool InMulti { get; set; }

        [JsonProperty("parent_streamer")]
        public object ParentStreamer { get; set; }

        public Settings Settings { get; set; }

        public string Viewers { get; set; }

        public string Username { get; set; }

        public List<object> Tags { get; set; }

        public User User { get; set; }

        public List<object> Recordings { get; set; }
    }

    public class Meta
    {
        [JsonProperty("limit_reached")]
        public bool LimitReached { get; set; }
    }
}