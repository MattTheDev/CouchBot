using System.Collections.Generic;
using System.Threading.Tasks;
using MTD.CouchBot.Domain.Dtos.Discord;

namespace MTD.CouchBot.Dals
{
    public interface IGroupDal
    {
        Task<GuildGroup> GetGuildGroupByGuildIdAndName(string guildId, string name);
        Task<List<GuildGroup>> GetGuildGroupsByGuildId(string guildId);
        Task CreateGuildGroup(GuildGroup guildGroup);
        Task UpdateGuildGroup(GuildGroup guildGroup);
        Task DeleteGuildGroup(GuildGroup guildGroup);
    }
}