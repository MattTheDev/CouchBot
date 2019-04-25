using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTD.CouchBot.Domain.Models.Twitch
{
    public class TwitchGameQueryResponse
    {
        public List<Game> data { get; set; }

        [JsonObject("Datum")]
        public class Game
        {
            public string id { get; set; }
            public string name { get; set; }
            public string box_art_url { get; set; }
        }
    }
}
