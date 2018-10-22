using System.Threading.Tasks;
using MTD.CouchBot.Domain.Dtos.Discord;

namespace MTD.CouchBot.Dals
{
    public interface IGuildDal
    {
        Task<Guild> GetGuildById(ulong id);
        Task<GuildConfiguration> GetGuildConfigurationByGuildId(ulong id);
        Task CreateGuild(Guild guild);
        Task CreateGuildConfiguration(GuildConfiguration guildConfiguration);
        Task UpdateGuildConfiguration(GuildConfiguration guildConfiguration);
        Task DeleteGuild(Guild guild);
        Task DeleteGuildConfiguration(GuildConfiguration guildConfiguration);
    }
}