using Newtonsoft.Json;

namespace MTD.CouchBot.Domain.Models.Mobcrush
{
    public class ChatRoom
    {
        [JsonProperty("_id")]
        public string Id { get; set; }
        [JsonProperty("metadata")]
        public Metadata Metadata { get; set; }
    }
}
