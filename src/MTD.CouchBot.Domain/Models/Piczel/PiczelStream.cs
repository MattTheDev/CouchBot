using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MTD.CouchBot.Domain.Models.Piczel
{
    public class PiczelStream
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        [JsonProperty("rendered_description ")]
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
        public PiczelOfflineImage OfflineImage { get; set; }
        public PiczelBanner Banner { get; set; }
        [JsonProperty("banner_link")]
        public object BannerLink { get; set; }
        public bool Adult { get; set; }
        [JsonProperty("in_multi")]
        public bool InMulti { get; set; }
        [JsonProperty("parent_streamer")]
        public string ParentStreamer { get; set; }
        public PiczelSettings Settings { get; set; }
        public int Viewers { get; set; }
        public string Username { get; set; }
        public List<object> Recordings { get; set; }
        public PiczelUser User { get; set; }
        public List<object> Tags { get; set; }
}
}
