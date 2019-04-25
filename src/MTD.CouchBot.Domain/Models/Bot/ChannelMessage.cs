namespace MTD.CouchBot.Domain.Models.Bot
{
    public class ChannelMessage
    {
        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }
        public ulong MessageId { get; set; }
        public bool DeleteOffline { get; set; }
    }
}
