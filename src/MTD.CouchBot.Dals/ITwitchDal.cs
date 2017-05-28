using MTD.CouchBot.Domain.Models;
using MTD.CouchBot.Domain.Models.Twitch;
using System.Threading.Tasks;

namespace MTD.CouchBot.Dals
{
    public interface ITwitchDal
    {
        Task<TwitchStreamV5> GetStreamById(string twitchId);
        Task<TwitchFollowers> GetFollowersByName(string name);
        Task<string> GetTwitchIdByLogin(string name);
        Task<TwitchStreamsV5> GetStreamsByIdList(string twitchIdList);
        Task<TwitchChannelFeed> GetChannelFeedPosts(string twitchId);
    }
}
