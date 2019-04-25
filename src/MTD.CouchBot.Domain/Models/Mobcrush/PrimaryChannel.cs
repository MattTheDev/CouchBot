using Newtonsoft.Json;

namespace MTD.CouchBot.Domain.Models.Mobcrush
{
    public class PrimaryChannel
    {
        [JsonProperty("chatroomObjectId")]
        public string ChatroomObjectId { get; set; }
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
    }
}
