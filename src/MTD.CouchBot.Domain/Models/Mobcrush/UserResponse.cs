using Newtonsoft.Json;
using System.Collections.Generic;

namespace MTD.CouchBot.Domain.Models.Mobcrush
{
    public class UserResponse
    {
        [JsonProperty("_id")]
        public string Id { get; set; }
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("enableDonations")]
        public bool EnableDonations { get; set; }
        [JsonProperty("likeCount")]
        public int LikeCount { get; set; }
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
        [JsonProperty("primaryChannel")]
        public PrimaryChannel PrimaryChannel { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("permissions")]
        public List<object> Permissions { get; set; }
        [JsonProperty("currentFollowed")]
        public bool CurrentFollowed { get; set; }
        [JsonProperty("viewCount")]
        public int ViewCount { get; set; }
        [JsonProperty("trustAllLinks")]
        public bool TrustAllLinks { get; set; }
        [JsonProperty("isLive")]
        public bool IsLive { get; set; }
        [JsonProperty("profileLogo")]
        public string ProfileLogo { get; set; }
    }
}
