using System.Collections.Generic;
using Newtonsoft.Json;

namespace MTD.CouchBot.Domain.Dtos.Twitch
{
    public class TwitchGameQueryResponse : ITwitchQueryResponse
    {
        [JsonProperty("data")]
        public List<Game> Games { get; set; }

        public class Game
        {
            public string Id { get; set; }
            public string Name { get; set; }
            [JsonProperty("box_art_url")]
            public string BoxArtUrl { get; set; }
        }
    }
}