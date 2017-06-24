using System.Collections.Generic;

namespace MTD.CouchBot.Domain.Models.Twitch
{
    public class TwitchGameSearchResponse
    {
        public List<Game> games { get; set; }

        public class Box
        {
            public string large { get; set; }
            public string medium { get; set; }
            public string small { get; set; }
            public string template { get; set; }
        }

        public class Logo
        {
            public string large { get; set; }
            public string medium { get; set; }
            public string small { get; set; }
            public string template { get; set; }
        }

        public class Game
        {
            public string name { get; set; }
            public int popularity { get; set; }
            public int _id { get; set; }
            public int giantbomb_id { get; set; }
            public Box box { get; set; }
            public Logo logo { get; set; }
            public string localized_name { get; set; }
            public string locale { get; set; }
        }
    }
}
