using MTD.DiscordBot.Domain.Models;
using Newtonsoft.Json;
using System.IO;

namespace MTD.CouchBot.Domain
{
    public static class Constants
    {
        static BotSettings _settings;

        public static BotSettings Settings
        {
            get
            {
                return JsonConvert.DeserializeObject<BotSettings>(File.ReadAllText(Constants.ConfigRootDirectory + Constants.BotSettings));
            }

            set
            {
                _settings = value;
            }
        }
        public static readonly string ConfigRootDirectory = @"c:\programdata\CouchBot\";
        public static readonly string GuildDirectory = @"Guilds\";
        public static readonly string UserDirectory = @"Users\";
        public static readonly string LiveDirectory = @"Live\";
        public static readonly string TwitchDirectory = @"Twitch\";
        public static readonly string YouTubeDirectory = @"YouTube\";
        public static readonly string BeamDirectory = @"Beam\";
        public static readonly string HitboxDirectory = @"Hitbox\";
        public static readonly string BotStatistics = "BotStats.json";
        public static readonly string BotSettings = "BotSettings.json";
        public static readonly string DiscordToken = Settings.DiscordToken;
        public static readonly string TwitchClientId = Settings.TwitchClientId;
        public static readonly string YouTubeApiKey = Settings.YouTubeApiKey;
        public static readonly ulong CouchBotId = Settings.CouchBotId;
    }
}
