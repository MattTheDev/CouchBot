namespace MTD.CouchBot.Domain.Models
{
    public class BotSettings
    { 
        public string DiscordToken { get; set; }
        public string TwitchClientId { get; set; }
        public string YouTubeApiKey { get; set; }
        public ulong CouchBotId { get; set; }
        public string Prefix { get; set; }
        public bool EnableBeam { get; set; }
        public bool EnableHitbox { get; set; }
        public bool EnableTwitch { get; set; }
        public bool EnableYouTube { get; set; }
    }
}
