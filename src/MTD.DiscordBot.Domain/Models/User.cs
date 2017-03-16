using MTD.DiscordBot.Domain.Models;
using System;

namespace MTD.DiscordBot.Json
{
    public class User
    {
        public ulong Id { get; set; }
        public string YouTubeChannelId { get; set; }
        public string TwitchName { get; set; }
        public string TwitchId { get; set; }
        public string BeamName { get; set; }
        public string HitboxName { get; set; }
        public string YouTubeSubGoal { get; set; }
        public bool YouTubeSubGoalMet { get; set; }
        public string TwitchFollowerGoal { get; set; }
        public bool TwitchFollowerGoalMet { get; set; }
        public string BeamFollowerGoal { get; set; }
        public bool BeamFollowerGoalMet { get; set; }
        public DateTime? Birthday { get; set; }
        public Schedule Schedule { get; set; }
        public float TimeZoneOffset { get; set; }
    }
}
