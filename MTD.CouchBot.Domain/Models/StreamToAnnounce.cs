using System.Collections.Generic;
using MTD.CouchBot.Domain.Dtos.Discord;
using MTD.CouchBot.Domain.Enums;

namespace MTD.CouchBot.Domain.Models
{
    public class StreamToAnnounce
    {
        public GuildGroup GuildGroups { get; set; }
        public Platform Platform { get; set; }
        public string GameId { get; set; }
        public string Game { get; set; }
        public string TeamId { get; set; }
        public string Team { get; set; }
        public List<string> TagIds { get; set; }
        public List<string> Tags { get; set; }
        public string DiscordChannelId { get; set; }
        public string CreatorChannelId { get; set; }
    }
}