using System;
using System.Collections.Generic;

namespace MTD.CouchBot.Domain.Models.Bot
{
    public class BotStats
    {
        public int YouTubeAlertCount { get; set; }
        public int TwitchAlertCount { get; set; }
        public int BeamAlertCount { get; set; }
        public int HitboxAlertCount { get; set; }
        public int PicartoAlertCount { get; set; }
        public int VidMeAlertCount { get; set; }
        public int UptimeMinutes { get; set; }
        public DateTime LoggingStartDate { get; set; }
        public DateTime LastRestart { get; set; }
        public List<int> BeamSubIds { get; set; }
        public int HaiBaiCount { get; set; }
        public int FlipCount { get; set; }
        public int UnflipCount { get; set; }
    }
}
