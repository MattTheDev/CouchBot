using MTD.CouchBot.Domain.Models;
using MTD.CouchBot.Domain.Models.Twitch;
using System.Collections.Generic;
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
        Task<TwitchTeam> GetTwitchTeamByName(string name);
        Task<string> GetDelimitedListOfTwitchMemberIds(string teamToken);
        Task<List<TwitchStreamsV5.Stream>> GetStreamsByGameName(string gameName);
        Task<TwitchGameSearchResponse> SearchForGameByName(string gameName);
        Task<TwitchChannelResponse> GetTwitchChannelById(string twitchId);
    }
}
