namespace MTD.DiscordBot.Domain.Models
{
    public class ChannelMessage
    {
        public ulong ChannelId { get; set; }
        public ulong MessageId { get; set; }
        public bool MarkOffline { get; set; }
    }
}
