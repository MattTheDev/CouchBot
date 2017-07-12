using MTD.CouchBot.Dals;
using MTD.CouchBot.Dals.Implementations;
using MTD.CouchBot.Domain.Models.Twitch;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MTD.CouchBot.Managers.Implementations
{
    public class TwitchManager : ITwitchManager
    {
        ITwitchDal _twitchDal;

        public TwitchManager()
        {
            _twitchDal = new TwitchDal();
        }

        public async Task<TwitchStreamV5> GetStreamById(string twitchId)
        {
            return await _twitchDal.GetStreamById(twitchId);
        }

        public async Task<TwitchFollowers> GetFollowersByName(string name)
        {
            return await _twitchDal.GetFollowersByName(name);
        }

        public async Task<string> GetTwitchIdByLogin(string name)
        {
            return await _twitchDal.GetTwitchIdByLogin(name);
        }

        public async Task<TwitchStreamsV5> GetStreamsByIdList(List<string> twitchIdList)
        {
            string list = "";
            foreach (var id in twitchIdList)
            {
                list += id + ",";
            }

            list = list.TrimEnd(',');

            return await _twitchDal.GetStreamsByIdList(list);
        }

        public async Task<TwitchStreamsV5> GetStreamsByIdList(string twitchIdList)
        {
            return await _twitchDal.GetStreamsByIdList(twitchIdList);
        }

        public async Task<TwitchChannelFeed> GetChannelFeedPosts(string twitchId)
        {
            return await _twitchDal.GetChannelFeedPosts(twitchId);
        }
        
        public async Task<TwitchTeam> GetTwitchTeamByName(string name)
        {
            return await _twitchDal.GetTwitchTeamByName(name);
        }

        public async Task<string> GetDelimitedListOfTwitchMemberIds(string teamToken)
        {
            return await _twitchDal.GetDelimitedListOfTwitchMemberIds(teamToken);
        }

        public async Task<List<TwitchStreamsV5.Stream>> GetStreamsByGameName(string gameName)
        {
            return await _twitchDal.GetStreamsByGameName(gameName);
        }

        public async Task<TwitchGameSearchResponse> SearchForGameByName(string gameName)
        {
            return await _twitchDal.SearchForGameByName(gameName);
        }
    }
}
