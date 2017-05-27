using Newtonsoft.Json;
using System.ComponentModel;

namespace MTD.CouchBot.Domain.Models
{
    public class BotSettings
    { 
        public string DiscordToken { get; set; }
        public string TwitchClientId { get; set; }
        public string YouTubeApiKey { get; set; }
        public ulong CouchBotId { get; set; }
        public string Prefix { get; set; }

        [DefaultValue(@"c:\programdata\CouchBot\")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public string ConfigRootDirectory { get; set; }
        [DefaultValue(@"Guilds\")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public string GuildDirectory { get; set; }
        [DefaultValue(@"Users\")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public string UserDirectory { get; set; }
        [DefaultValue(@"Live\")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public string LiveDirectory { get; set; }
        [DefaultValue(@"Beam\")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public string BeamDirectory { get; set; }
        [DefaultValue(@"Smashcast\")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public string SmashcastDirectory { get; set; }
        [DefaultValue(@"Twitch\")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public string TwitchDirectory { get; set; }
        [DefaultValue(@"YouTube\")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public string YouTubeDirectory { get; set; }
        public string PicartoDirectory { get; set; }

        [DefaultValue(true)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool EnableMixer { get; set; }
        [DefaultValue(true)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool EnableSmashcast { get; set; }
        [DefaultValue(true)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool EnableTwitch { get; set; }
        [DefaultValue(true)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool EnableYouTube { get; set; }
        public bool EnablePicarto { get; set; }
    }
}
