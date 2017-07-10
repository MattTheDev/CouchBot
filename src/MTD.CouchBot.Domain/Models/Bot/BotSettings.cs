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
            public string ApiAiKey { get; set; }
        }

        public class BotConfiguration
        {
            public ulong CouchBotId { get; set; }
            public string Prefix { get; set; }
            public int TotalShards { get; set; }
            public ulong OwnerId { get; set; }
        }

        public class Directories
        {
            public string ConfigRootDirectory { get; set; }
            public string GuildDirectory { get; set; }
            public string UserDirectory { get; set; }
            public string LiveDirectory { get; set; }
            public string MixerDirectory { get; set; }
            public string SmashcastDirectory { get; set; }
            public string TwitchDirectory { get; set; }
            public string YouTubeDirectory { get; set; }
            public string PicartoDirectory { get; set; }
        }

        public class Platforms
        {
            public bool EnableMixer { get; set; }
            public bool EnablePicarto { get; set; }
            public bool EnableSmashcast { get; set; }
            public bool EnableTwitch { get; set; }
            public bool EnableYouTube { get; set; }
            public bool EnableVidMe { get; set; }
        }

        public class Intervals
        {
            public int Picarto { get; set; }
            public int Smashcast { get; set; }
            public int Twitch { get; set; }
            public int TwitchFeed { get; set; }
            public int YouTubePublished { get; set; }
            public int YouTubeLive { get; set; }
            public int VidMe { get; set; }
        }
    }
}
