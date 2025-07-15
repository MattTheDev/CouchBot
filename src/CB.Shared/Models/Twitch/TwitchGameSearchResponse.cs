using Newtonsoft.Json;

namespace CB.Shared.Models.Twitch;

public class TwitchGameSearchResponse
{
    public List<Datum> Data { get; set; }

    public class Datum
    {
        [JsonProperty("box_art_url")]
        public string BoxArtUrl { get; set; }

        public string Id { get; set; }
        
        public string Name { get; set; }
    }
}