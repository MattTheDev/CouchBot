using MTD.CouchBot.Domain.Models.Twitch;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MTD.CouchBot.Managers
{
    public interface ITwitchManager
    {
        Task<TwitchStreamV5> GetStreamById(string twitchId);
        Task<TwitchFollowers> GetFollowersByName(string name);
        Task<string> GetTwitchIdByLogin(string name);
        Task<TwitchStreamsV5> GetStreamsByIdList(List<string> twitchIdList);
        Task<TwitchChannelFeed> GetChannelFeedPosts(string twitchId);
    }
}
