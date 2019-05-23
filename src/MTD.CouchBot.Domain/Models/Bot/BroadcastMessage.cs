using Discord;

namespace MTD.CouchBot.Domain.Models.Bot
{
    public class BroadcastMessage
    {
        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }
        public string UserId { get; set; }
        public string Message { get; set; }
        public string Platform { get; set; }
        public Embed Embed { get; set; }
        public bool DeleteOffline { get; set; }
    }
}
