using Newtonsoft.Json;

namespace MTD.CouchBot.Domain.Models.Piczel
{
    public class PiczelMeta
    {
        [JsonProperty("limit_reached")]
        public bool LimitReached { get; set; }
        [JsonProperty("host_id")]
        public int HostId { get; set; }
        [JsonProperty("host_name")]
        public string HostName { get; set; }
    }
}
