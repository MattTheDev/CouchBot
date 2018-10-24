using System.Collections.Generic;
using System.Threading.Tasks;
using MTD.CouchBot.Domain.Dtos.Discord;
using MTD.CouchBot.Domain.Enums;

namespace MTD.CouchBot.Managers
{
    public interface IChannelManager
    {
        Task<List<GuildGroupChannel>> GetAllChannelsByGuildGroupId(int guildGroupId);
        Task<List<GuildGroupChannel>> GetChannelsByGuildGroupIdAndPlatform(int guildGroupId, Platform platform);
        Task<List<GuildGroupChannel>> GetChannelByChannelId(string channelId);
        Task<GuildGroupChannel> GetChannelByGuildGroupIdAndChannelId(int guildGroupId, string channelId);
        Task AddChannel(GuildGroupChannel channel);
        Task RemoveChannel(GuildGroupChannel channel);
    }
}