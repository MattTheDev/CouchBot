using Newtonsoft.Json;
using System.Collections.Generic;

namespace MTD.CouchBot.Domain.Models.Piczel
{
    public class PiczelStreamResponse
    {
        public string Type { get; set; }

        [JsonProperty("data")]
        public List<PiczelStream> Streams { get; set; }

        public PiczelMeta Meta { get; set; }
    }
}
