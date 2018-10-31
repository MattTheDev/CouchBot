using MTD.CouchBot.Dals;
using MTD.CouchBot.Domain.Dtos.Discord;
using MTD.CouchBot.Domain.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MTD.CouchBot.Managers.Implementations
{
    public class GroupManager : IGroupManager
    {
        private readonly IGroupDal _groupDal;

        public GroupManager(IGroupDal groupDal)
        {
            _groupDal = groupDal;
        }

        public async Task<List<GuildGroup>> GetAllGuildGroups()
        {
            return await _groupDal.GetAllGuildGroups();
        }

        public async Task<List<GuildGroup>> GetAllGuildGroupsWithGroupChannels()
        {
            return await _groupDal.GetAllGuildGroupsWithGroupChannels();
        }

        public async Task<GuildGroup> GetGuildGroupByGuildIdAndName(ulong guildId, string name)
        {
            return await _groupDal.GetGuildGroupByGuildIdAndName(Cryptography.Encrypt(guildId.ToString()), name);
        }

        public async Task<List<GuildGroup>> GetGuildGroupsByGuildId(ulong guildId)
        {
            return await _groupDal.GetGuildGroupsByGuildId(Cryptography.Encrypt(guildId.ToString()));
        }

        public async Task CreateGuildGroup(GuildGroup guildGroup)
        {
            await _groupDal.CreateGuildGroup(guildGroup);
        }

        public async Task UpdateGuildGroup(GuildGroup guildGroup)
        {
            await _groupDal.UpdateGuildGroup(guildGroup);
        }

        public async Task DeleteGuildGroup(GuildGroup guildGroup)
        {
            await _groupDal.DeleteGuildGroup(guildGroup);
        }
    }
}