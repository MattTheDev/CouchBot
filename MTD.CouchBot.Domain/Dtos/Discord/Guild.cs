using System;

namespace MTD.CouchBot.Domain.Dtos.Discord
{
    public class Guild
    {
        public int Id { get; set; }
        public string GuildId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string OwnerId { get; set; }
    }
}