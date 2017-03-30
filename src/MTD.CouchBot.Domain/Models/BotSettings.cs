using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTD.DiscordBot.Domain.Models
{
    public class BotSettings
    {
        public string ConfigRootDirectory { get; set; }
        public string GuildDirectory { get; set; }
        public string UserDirectory { get; set; }
        public string LiveDirectory { get; set; }
        public string TwitchDirectory { get; set; }
        public string YouTubeDirectory { get; set; }
        public string BeamDirectory { get; set; }
        public string HitboxDirectory { get; set; }
        public string BotStatistics { get; set; }
        public string DiscordToken { get; set; }
        public string TwitchClientId { get; set; }
        public string YouTubeApiKey { get; set; }
        public ulong CouchBotId { get; set; }
    }
}
