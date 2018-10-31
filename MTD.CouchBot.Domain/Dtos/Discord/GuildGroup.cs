using DapperExtensions.Mapper;
using System.Collections.Generic;
using MTD.CouchBot.Domain.Utilities;

namespace MTD.CouchBot.Domain.Dtos.Discord
{
    public class GuildGroup
    {
        public int Id { get; set; }
        public string GuildId { get; set; }
        public string Name { get; set; }
        public string StreamChannelId { get; set; }
        public string VodChannelId { get; set; }
        public string MentionRoleId { get; set; }
        public string LiveMessage { get; set; }
        public string VodMessage { get; set; }

        public List<GuildGroupChannel> Channels { get; set; }
    }

    // ReSharper disable once UnusedMember.Global
    public class GuildGroupMapper : ClassMapper<GuildGroup>
    {
        public GuildGroupMapper()
        {
            Table("guildgroups");
            Map(gg => gg.Channels).Ignore();
            AutoMap();
        }
    }
}