using Discord;

namespace MTD.DiscordBot.Models
{
    public class BroadcastMessage
    {
        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }
        public ulong UserId { get; set; }
        public string Message { get; set; }
        public string Platform { get; set; }
        public Embed Embed { get; set; }
    }
}
