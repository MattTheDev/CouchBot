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

        public static readonly string BotStatistics = "BotStats.json";
        public static readonly string BotSettings = "BotSettings.json";

        public static readonly string ConfigRootDirectory = Settings.DirectorySettings.ConfigRootDirectory;
        public static readonly string GuildDirectory = Settings.DirectorySettings.GuildDirectory;
        public static readonly string UserDirectory = Settings.DirectorySettings.UserDirectory;
        public static readonly string LiveDirectory = Settings.DirectorySettings.LiveDirectory;
        public static readonly string TwitchDirectory = Settings.DirectorySettings.TwitchDirectory;
        public static readonly string YouTubeDirectory = Settings.DirectorySettings.YouTubeDirectory;
        public static readonly string MixerDirectory = Settings.DirectorySettings.MixerDirectory;
        public static readonly string SmashcastDirectory = Settings.DirectorySettings.SmashcastDirectory;
        public static readonly string PicartoDirectory = Settings.DirectorySettings.PicartoDirectory;

        public static readonly bool EnableMixer = Settings.PlatformSettings.EnableMixer;
        public static readonly bool EnableSmashcast = Settings.PlatformSettings.EnableSmashcast;
        public static readonly bool EnableTwitch = Settings.PlatformSettings.EnableTwitch;
        public static readonly bool EnableYouTube = Settings.PlatformSettings.EnableYouTube;
        public static readonly bool EnablePicarto = Settings.PlatformSettings.EnablePicarto;
        public static readonly bool EnableVidMe = Settings.PlatformSettings.EnableVidMe;

        public static readonly string DiscordToken = Settings.KeySettings.DiscordToken;
        public static readonly string TwitchClientId = Settings.KeySettings.TwitchClientId;
        public static readonly string YouTubeApiKey = Settings.KeySettings.YouTubeApiKey;
        public static readonly string ApiAiKey = Settings.KeySettings.ApiAiKey;
        public static readonly ulong CouchBotId = Settings.BotConfig.CouchBotId;
        public static readonly string Prefix = Settings.BotConfig.Prefix;
        public static readonly int TotalShards = Settings.BotConfig.TotalShards;
        public static readonly ulong OwnerId = Settings.BotConfig.OwnerId;

        public static readonly int PicartoInterval = Settings.IntervalSettings.Picarto * 1000;
        public static readonly int TwitchInterval = Settings.IntervalSettings.Twitch * 1000;
        public static readonly int TwitchFeedInterval = 120 * 1000;
        public static readonly int SmashcastInterval = Settings.IntervalSettings.Smashcast * 1000;
        public static readonly int YouTubePublishedInterval = Settings.IntervalSettings.YouTubePublished * 1000;
        public static readonly int YouTubeLiveInterval = Settings.IntervalSettings.YouTubeLive * 1000;
        public static readonly int VidMeInterval = Settings.IntervalSettings.VidMe * 1000;

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
        public static readonly string VidMe = "VidMe";
    }
}
