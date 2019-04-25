using Newtonsoft.Json;

namespace MTD.CouchBot.Domain.Models.Mobcrush
{
    public class Channel3
    {
        [JsonProperty("_id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("memberCount")]
        public int MemberCount { get; set; }
        [JsonProperty("totalViews")]
        public int TotalViews { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("offlinePosterImage")]
        public string OfflinePosterImage { get; set; }
    }
}
