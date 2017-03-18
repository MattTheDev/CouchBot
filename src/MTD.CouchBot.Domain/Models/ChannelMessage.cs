namespace MTD.CouchBot.Domain.Models
{
    public class ChannelMessage
    {
        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }
        public ulong MessageId { get; set; }
        public bool DeleteOffline { get; set; }
    }
}
