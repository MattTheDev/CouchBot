using Newtonsoft.Json;

namespace MTD.CouchBot.Domain.Models.Mobcrush
{
    public class User
    {
        [JsonProperty("_id")]
        public string Id { get; set; }
        [JsonProperty("channel")]
        public Channel Channel { get; set; }
        //[JsonProperty("user")]
        //public User2 User2 { get; set; }
        [JsonProperty("user")]
        public User3 User3 { get; set; }
        [JsonProperty("chatRoom")]
        public string ChatRoom { get; set; }
        [JsonProperty("chatroomObjectId")]
        public string ChatroomObjectId { get; set; }
        [JsonProperty("lastBroadcast")]
        public LastBroadcast LastBroadcast { get; set; }
        [JsonProperty("isLive")]
        public bool IsLive { get; set; }
    }
}
