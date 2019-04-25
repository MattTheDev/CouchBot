using Newtonsoft.Json;

namespace MTD.CouchBot.Domain.Models.Mobcrush
{
    public class Urls
    {
        [JsonProperty("ios")]
        public string Ios { get; set; }
        [JsonProperty("android")]
        public string Android { get; set; }
    }
}
