using MTD.CouchBot.Domain.Dtos.Discord;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MTD.CouchBot.Dals
{
    public interface IGroupDal
    {
        Task<List<GuildGroup>> GetAllGuildGroups();
        Task<List<GuildGroup>> GetAllGuildGroupsWithGroupChannels();
        Task<GuildGroup> GetGuildGroupByGuildIdAndName(string guildId, string name);
        Task<List<GuildGroup>> GetGuildGroupsByGuildId(string guildId);
        Task CreateGuildGroup(GuildGroup guildGroup);
        Task UpdateGuildGroup(GuildGroup guildGroup);
        Task DeleteGuildGroup(GuildGroup guildGroup);
    }
}