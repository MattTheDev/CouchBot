using MTD.CouchBot.Dals;
using MTD.CouchBot.Domain.Dtos.Discord;
using System.Threading.Tasks;

namespace MTD.CouchBot.Managers.Implementations
{
    public class GuildManager : IGuildManager
    {
        private readonly IGuildDal _guildDal;

        public GuildManager(IGuildDal guildDal)
        {
            _guildDal = guildDal;
        }

        public async Task<GuildConfiguration> GetGuildConfigurationByGuildId(ulong id)
        {
            return await _guildDal.GetGuildConfigurationByGuildId(id);
        }

        public async Task CreateGuild(Guild guild)
        {
            await _guildDal.CreateGuild(guild);
        }

        public async Task CreateGuildConfiguration(GuildConfiguration guildConfiguration)
        {
            await _guildDal.CreateGuildConfiguration(guildConfiguration);
        }

        public async Task UpdateGuildConfiguration(GuildConfiguration guildConfiguration)
        {
            await _guildDal.UpdateGuildConfiguration(guildConfiguration);
        }

        public async Task<Guild> GetGuildById(ulong id)
        {
            return await _guildDal.GetGuildById(id);
        }

        public async Task DeleteGuild(Guild guild)
        {
            await _guildDal.DeleteGuild(guild);
        }

        public async Task DeleteGuildConfiguration(GuildConfiguration guildConfiguration)
        {
            await _guildDal.DeleteGuildConfiguration(guildConfiguration);
        }
    }
}