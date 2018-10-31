using System.Collections.Generic;
using System.Threading.Tasks;
using MTD.CouchBot.Dals;
using MTD.CouchBot.Domain.Dtos.Discord;
using MTD.CouchBot.Domain.Enums;
using MTD.CouchBot.Domain.Utilities;

namespace MTD.CouchBot.Managers.Implementations
{
    public class ChannelManager : IChannelManager
    {
        private readonly IChannelDal _channelDal;

        public ChannelManager(IChannelDal channelDal)
        {
            _channelDal = channelDal;
        }

        public async Task<List<GuildGroupChannel>> GetAllChannelsByGuildGroupId(int guildGroupId)
        {
            return await _channelDal.GetAllChannelsByGuildGroupId(guildGroupId);
        }

        public async Task<List<GuildGroupChannel>> GetChannelsByGuildGroupIdAndPlatform(int guildGroupId, Platform platform)
        {
            return await _channelDal.GetChannelsByGuildIdAndPlatform(guildGroupId,
                platform);
        }

        public async Task<List<GuildGroupChannel>> GetChannelByChannelId(string channelId)
        {
            return await _channelDal.GetChannelByChannelId(channelId);
        }

        public async Task<GuildGroupChannel> GetChannelByGuildGroupIdAndChannelId(int guildGroupId, string channelId)
        {
            return await _channelDal.GetChannelByGuildIdAndChannelId(guildGroupId,
                channelId);
        }

        public async Task AddChannel(GuildGroupChannel channel)
        {
            await _channelDal.AddChannel(channel);
        }

        public async Task RemoveChannel(GuildGroupChannel channel)
        {
            await _channelDal.RemoveChannel(channel);
        }
    }
}