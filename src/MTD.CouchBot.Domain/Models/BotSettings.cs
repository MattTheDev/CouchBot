using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTD.CouchBot.Domain.Models
{
    public class BotSettings
    {
        public string DiscordToken { get; set; }
        public string TwitchClientId { get; set; }
        public string YouTubeApiKey { get; set; }
        public ulong CouchBotId { get; set; }
    }
}
