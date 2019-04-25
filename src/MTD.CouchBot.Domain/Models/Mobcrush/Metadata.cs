using Newtonsoft.Json;

namespace MTD.CouchBot.Domain.Models.Mobcrush
{
    public class Metadata
    {
        [JsonProperty("broadcastId")]
        public string BroadcastId { get; set; }
    }
}
