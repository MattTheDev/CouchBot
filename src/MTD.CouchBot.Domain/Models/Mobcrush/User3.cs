using Newtonsoft.Json;

namespace MTD.CouchBot.Domain.Models.Mobcrush
{
    public class User3
    {
        [JsonProperty("_id")]
        public string Id { get; set; }
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("followerCount")]
        public int FollowerCount { get; set; }
        [JsonProperty("primaryChannelUser")]
        public string PrimaryChannelUser { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("profileLogo")]
        public string ProfileLogo { get; set; }
        [JsonProperty("primaryChannel")]
        public PrimaryChannel PrimaryChannel { get; set; }
        [JsonProperty("objevId")]
        public string ObjevId { get; set; }
        [JsonProperty("whisperChatroomObjectId")]
        public string WhisperChatroomObjectId { get; set; }
        [JsonProperty("isPartner")]
        public bool IsPartner { get; set; }
    }
}
