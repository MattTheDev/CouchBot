using Newtonsoft.Json;

namespace MTD.CouchBot.Domain.Models.Mobcrush
{
    public class User2
    {
        [JsonProperty("_id")]
        public string Id { get; set; }
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("enableDonations")]
        public bool EnableDonations { get; set; }
        [JsonProperty("broadcastCount")]
        public int BroadcastCount { get; set; }
        [JsonProperty("followingCount")]
        public int FollowingCount { get; set; }
        [JsonProperty("followerCount")]
        public int FollowerCount { get; set; }
        [JsonProperty("objevId")]
        public string ObjevId { get; set; }
        [JsonProperty("whisperChatroomObjectId")]
        public string WhisperChatroomObjectId { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("trustAllLinks")]
        public bool TrustAllLinks { get; set; }
    }
}
