using System.Collections.Generic;

namespace MTD.CouchBot.Domain.Models.Twitch
{
    public class TwitchGameServerModel
    {
        public string Name { get; set; }
        public List<ulong> Servers { get; set; }
    }
}
