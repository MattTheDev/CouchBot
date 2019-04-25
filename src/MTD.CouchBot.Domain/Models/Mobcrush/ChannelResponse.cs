using Newtonsoft.Json;
using System.Collections.Generic;

namespace MTD.CouchBot.Domain.Models.Mobcrush
{
    public class ChannelResponse
    {
        [JsonProperty("channel")]
        public Channel Channel { get; set; }
        [JsonProperty("users")]
        public List<User> Users { get; set; }
    }
}
