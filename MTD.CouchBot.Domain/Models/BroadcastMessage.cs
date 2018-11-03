using Discord;
using MTD.CouchBot.Domain.Enums;

namespace MTD.CouchBot.Domain.Models
{
    public class BroadcastMessage
    {
        public string GuildId { get; set; }
        public string ChannelId { get; set; }
        public string CreatorChannelID { get; set; }
        public string Message { get; set; }
        public Platform Platform { get; set; }
        public Embed Embed { get; set; }
        public bool DeleteOffline { get; set; }
    }
}