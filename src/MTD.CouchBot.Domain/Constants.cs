using Discord;
using MTD.CouchBot.Domain.Models.Bot;
using MTD.CouchBot.Domain.Utilities;
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
                return JsonConvert.DeserializeObject<BotSettings>(File.ReadAllText(BotSettings));
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
        public static readonly string MixerDirectory = @"Mixer\";
        public static readonly string SmashcastDirectory = @"Smashcast\";
        public static readonly string PicartoDirectory = @"Picarto\";

        public static readonly string BotStatistics = "BotStats.json";
        public static readonly string BotSettings = "BotSettings.json";

        public static readonly bool EnableMixer = Settings.PlatformSettings.EnableMixer;
        public static readonly bool EnableSmashcast = Settings.PlatformSettings.EnableSmashcast;
        public static readonly bool EnableTwitch = Settings.PlatformSettings.EnableTwitch;
        public static readonly bool EnableYouTube = Settings.PlatformSettings.EnableYouTube;
        public static readonly bool EnablePicarto = Settings.PlatformSettings.EnablePicarto;

        public static readonly string DiscordToken = Settings.KeySettings.DiscordToken;
        public static readonly string TwitchClientId = Settings.KeySettings.TwitchClientId;
        public static readonly string YouTubeApiKey = Settings.KeySettings.YouTubeApiKey;
        public static readonly ulong CouchBotId = Settings.BotConfig.CouchBotId;
        public static readonly string Prefix = Settings.BotConfig.Prefix;
        public static readonly int TotalShards = Settings.BotConfig.TotalShards;

        public static readonly int PicartoInterval = Settings.IntervalSettings.Picarto * 1000;
        public static readonly int TwitchInterval = Settings.IntervalSettings.Twitch * 1000;
        public static readonly int TwitchFeedInterval = Settings.IntervalSettings.TwitchFeed * 1000;
        public static readonly int SmashcastInterval = Settings.IntervalSettings.Smashcast * 1000;
        public static readonly int YouTubePublishedInterval = Settings.IntervalSettings.YouTubePublished * 1000;
        public static readonly int YouTubeLiveInterval = Settings.IntervalSettings.YouTubeLive * 1000;

        public static readonly Color Blue = new Color(76, 144, 243);
        public static readonly Color Red = new Color(179, 18, 23);
        public static readonly Color Purple = new Color(100, 65, 164);
        public static readonly Color Green = new Color(153, 204, 0);

        public static readonly string Mixer = "Mixer";
        public static readonly string Smashcast = "Smashcast";
        public static readonly string Twitch = "Twitch";
        public static readonly string YouTubeGaming = "YouTube Gaming";
        public static readonly string YouTube = "YouTube";
        public static readonly string Picarto = "Picarto";
    }
}
