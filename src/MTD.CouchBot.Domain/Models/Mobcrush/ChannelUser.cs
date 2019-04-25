using Newtonsoft.Json;

namespace MTD.CouchBot.Domain.Models.Mobcrush
{
    public class ChannelUser
    {
        [JsonProperty("_id")]
        public string Id { get; set; }
        [JsonProperty("chatRoom")]
        public string ChatRoom { get; set; }
        [JsonProperty("chatroomObjectId")]
        public string ChatroomObjectId { get; set; }
    }
}
