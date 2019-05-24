using MTD.CouchBot.Domain.Enums;

namespace MTD.CouchBot.Domain.Models.Bot
{
    public class DiscordStreamer
    {
        public ulong DisordChannelId { get; set; }
        public string StreamerChannelId { get; set; }
        public Platform Platform { get; set; }
    }
}