using System.Collections.Generic;

namespace MTD.CouchBot.Domain.Models.Bot
{
    public class LiveChannel
    {
        public string Name { get; set; }
        public List<ulong> Servers { get; set; }
        public List<ChannelMessage> ChannelMessages {get;set;}
        public ulong? DiscordUserId { get; set; }
    }
}
