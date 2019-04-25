using Newtonsoft.Json;
using System.Collections.Generic;

namespace MTD.CouchBot.Domain.Models.Mobcrush
{
    public class ChannelBroadcastResponse
    {
        [JsonProperty("_id")]
        public string Id { get; set; }
        [JsonProperty("ingestIndex")]
        public int IngestIndex { get; set; }
        [JsonProperty("regionName")]
        public string RegionName { get; set; }
        [JsonProperty("startDate")]
        public string StartDate { get; set; }
        [JsonProperty("channelUser")]
        public ChannelUser ChannelUser { get; set; }
        [JsonProperty("game")]
        public Game Game { get; set; }
        [JsonProperty("user")]
        public User User { get; set; }
        [JsonProperty("channel")]
        public Channel Channel { get; set; }
        [JsonProperty("storageFormats")]
        public List<string> StorageFormats { get; set; }
        [JsonProperty("mature")]
        public bool Mature { get; set; }
        [JsonProperty("hasCustomThumbnail")]
        public bool HasCustomThumbnail { get; set; }
        [JsonProperty("totalViews")]
        public int TotalViews { get; set; }
        [JsonProperty("currentViewers")]
        public int CurrentViewers { get; set; }
        [JsonProperty("likes")]
        public int Likes { get; set; }
        [JsonProperty("height")]
        public int Height { get; set; }
        [JsonProperty("width")]
        public int Width { get; set; }
        [JsonProperty("isLive")]
        public bool IsLive { get; set; }
        [JsonProperty("chatRoom")]
        public ChatRoom ChatRoom { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("endDate")]
        public string EndDate { get; set; }
    }
}
