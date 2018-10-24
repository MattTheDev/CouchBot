using System.Collections.Generic;
using System.Threading.Tasks;
using MTD.CouchBot.Domain.Dtos.Discord;
using MTD.CouchBot.Domain.Enums;

namespace MTD.CouchBot.Dals
{
    public interface IChannelDal
    {
        Task<List<GuildGroupChannel>> GetAllChannelsByGuildId(int guildGroupId);
        Task<List<GuildGroupChannel>> GetChannelsByGuildIdAndPlatform(int guildGroupId, Platform platform);
        Task<List<GuildGroupChannel>> GetChannelByChannelId(string channelId);
        Task<GuildGroupChannel> GetChannelByGuildIdAndChannelId(int guildGroupId, string channelId);
        Task AddChannel(GuildGroupChannel channel);
        Task RemoveChannel(GuildGroupChannel channel);
    }
}