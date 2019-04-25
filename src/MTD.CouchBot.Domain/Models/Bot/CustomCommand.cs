using System;

namespace MTD.CouchBot.Domain.Models.Bot
{
    public class CustomCommand
    {
        public string Command { get; set; }
        public string Output { get; set; }
        public int Cooldown { get; set; }
        public DateTime LastRun { get; set; }
        public bool Repeat { get; set; }
        public int Interval { get; set; }
        public ulong ChannelId { get; set; }
    }
}
