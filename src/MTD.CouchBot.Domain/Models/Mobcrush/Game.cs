using Newtonsoft.Json;

namespace MTD.CouchBot.Domain.Models.Mobcrush
{
    public class Game
    {
        [JsonProperty("_id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("icon")]
        public string Icon { get; set; }
        [JsonProperty("androidPackageName")]
        public string AndroidPackageName { get; set; }
        [JsonProperty("lowerUrl")]
        public string LowerUrl { get; set; }
        [JsonProperty("itunesId")]
        public int ItunesId { get; set; }
        [JsonProperty("urls")]
        public Urls Urls { get; set; }
    }
}
