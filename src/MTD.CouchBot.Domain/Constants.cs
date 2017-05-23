using MTD.CouchBot.Domain.Utilities;
using MTD.CouchBot.Domain.Models;
using Newtonsoft.Json;
using System.IO;
using Discord;

namespace MTD.CouchBot.Domain
{
    public static class Constants
    {
        static BotSettings _settings;

        public static BotSettings Settings
        {
            get
            {
                BotFiles.CheckConfiguration();
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

        public static readonly bool EnableBeam = Settings.EnableBeam;
        public static readonly bool EnableSmashcast = Settings.EnableSmashcast;
        public static readonly bool EnableTwitch = Settings.EnableTwitch;
        public static readonly bool EnableYouTube = Settings.EnableYouTube;

        public static readonly string DiscordToken = Settings.DiscordToken;
        public static readonly string TwitchClientId = Settings.TwitchClientId;
        public static readonly string YouTubeApiKey = Settings.YouTubeApiKey;
        public static readonly ulong CouchBotId = Settings.CouchBotId;
        public static readonly string Prefix = Settings.Prefix;

        public static readonly Color Blue = new Color(76, 144, 243);
        public static readonly Color Red = new Color(179, 18, 23);
        public static readonly Color Purple = new Color(100, 65, 164);
        public static readonly Color Green = new Color(153, 204, 0);

        public static readonly string Beam = "Beam";
        public static readonly string Smashcast = "Smashcast";
        public static readonly string Twitch = "Twitch";
        public static readonly string YouTubeGaming = "YouTube Gaming";
        public static readonly string YouTube = "YouTube";

    }
}
