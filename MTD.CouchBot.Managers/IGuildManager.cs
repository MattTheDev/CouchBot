using MTD.CouchBot.Domain.Dtos.Discord;
using System.Threading.Tasks;

namespace MTD.CouchBot.Managers
{
    public interface IGuildManager
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