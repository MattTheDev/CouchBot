using MTD.CouchBot.Domain.Enums;

namespace MTD.CouchBot.Domain.Dtos.Discord
{
    public class GuildGroupChannel
    {
        public int Id { get; set; }
        public int GuildGroupId { get; set; }
        public Platform Platform { get; set; }
        public string ChannelId { get; set; }
    }
}