using System;

namespace MTD.DiscordBot.Domain.Models
{
    public class BotStats
    {
        public int YouTubeAlertCount { get; set; }
        public int TwitchAlertCount { get; set; }
        public int BeamAlertCount { get; set; }
        public int HitboxAlertCount { get; set; }
        public int UptimeMinutes { get; set; }
        public DateTime LoggingStartDate { get; set; }
        public DateTime LastRestart { get; set; }
    }
}
