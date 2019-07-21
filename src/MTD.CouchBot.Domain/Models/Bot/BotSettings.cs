using Newtonsoft.Json;

namespace MTD.CouchBot.Domain.Models.Bot
{
    public class BotSettings
    {
        [JsonProperty("Keys")]
        public Keys KeySettings { get; set; }
        [JsonProperty("BotConfiguration")]
        public BotConfiguration BotConfig { get; set; }
        [JsonProperty("Directories")]
        public Directories DirectorySettings { get; set; }
        [JsonProperty("Platforms")]
        public Platforms PlatformSettings { get; set; }
        [JsonProperty("Intervals")]
        public Intervals IntervalSettings { get; set; }

        public class Keys
        {
            public string DiscordToken { get; set; }
            public string TwitchClientId { get; set; }
            public string YouTubeApiKey { get; set; }
            public string MixerClientId { get; set; }
        }

        public class BotConfiguration
        {
            public ulong CouchBotId { get; set; }
            public string Prefix { get; set; }
            public int TotalShards { get; set; }
            public ulong OwnerId { get; set; }
            public bool EnableSendingMessages { get; set; }
            public ulong DiscordErrorChannelId { get; set; }
            public ulong DiscordAuditChannelId { get; set; }
            public bool EnableCustomTimerCommands { get; set; }
            public bool EnableChannelLogging { get; set; }
            public ulong JoinAndLeaveFeedChannelId { get; set; }
        }

        public class Directories
        {
            public string ConfigRootDirectory { get; set; }
            public string GuildDirectory { get; set; }
            public string LiveDirectory { get; set; }
            public string MixerDirectory { get; set; }
            public string SmashcastDirectory { get; set; }
            public string TwitchDirectory { get; set; }
            public string YouTubeDirectory { get; set; }
            public string PicartoDirectory { get; set; }
            public string MobcrushDirectory { get; set; }
            public string PiczelDirectory { get; set; }
        }

        public class Platforms
        {
            public bool EnableMixer { get; set; }
            public bool EnablePicarto { get; set; }
            public bool EnableSmashcast { get; set; }
            public bool EnableTwitch { get; set; }
            public bool EnableYouTube { get; set; }
            public bool EnableMobcrush { get; set; }
            public bool EnablePiczel { get; set; }
        }

        public class Intervals
        {
            public int Picarto { get; set; }
            public int Piczel { get; set; }
            public int Smashcast { get; set; }
            public int Twitch { get; set; }
            public int YouTubePublished { get; set; }
            public int YouTubeLive { get; set; }
            public int TwitchServer { get; set; }
            public int Mobcrush { get; set; }
        }
    }
}
