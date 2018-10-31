using MTD.CouchBot.Domain.Dtos.Discord;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MTD.CouchBot.Managers
{
    public interface IGroupManager
    {
        Task<List<GuildGroup>> GetAllGuildGroups();
        Task<List<GuildGroup>> GetAllGuildGroupsWithGroupChannels();
        Task<GuildGroup> GetGuildGroupByGuildIdAndName(ulong guildId, string name);
        Task<List<GuildGroup>> GetGuildGroupsByGuildId(ulong guildId);
        Task CreateGuildGroup(GuildGroup guildGroup);
        Task UpdateGuildGroup(GuildGroup guildGroup);
        Task DeleteGuildGroup(GuildGroup guildGroup);
    }
}