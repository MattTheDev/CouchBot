using MTD.CouchBot.Dals;
using MTD.CouchBot.Dals.Implementations;
using MTD.CouchBot.Domain.Models.Twitch;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace MTD.CouchBot.Managers.Implementations
{
    public class TwitchManager : ITwitchManager
    {
        ITwitchDal twitchDal;

        public TwitchManager()
        {
            twitchDal = new TwitchDal();
        }

        public async Task<TwitchStreamV5> GetStreamById(string twitchId)
        {
            return await twitchDal.GetStreamById(twitchId);
        }

        public async Task<TwitchFollowers> GetFollowersByName(string name)
        {
            return await twitchDal.GetFollowersByName(name);
        }

        public async Task<string> GetTwitchIdByLogin(string name)
        {
            return await twitchDal.GetTwitchIdByLogin(name);
        }

        public async Task<TwitchStreamsV5> GetStreamsByIdList(List<string> twitchIdList)
        {
            string list = "";
            foreach (var id in twitchIdList)
            {
                list += id + ",";
            }

            list = list.TrimEnd(',');

            return await twitchDal.GetStreamsByIdList(list);
        }

        public async Task<TwitchStreamsV5> GetStreamsByIdList(string twitchIdList)
        {
            return await twitchDal.GetStreamsByIdList(twitchIdList);
        }

        public async Task<TwitchChannelFeed> GetChannelFeedPosts(string twitchId)
        {
            return await twitchDal.GetChannelFeedPosts(twitchId);
        }
        
        public async Task<TwitchTeam> GetTwitchTeamByName(string name)
        {
            return await twitchDal.GetTwitchTeamByName(name);
        }

        public async Task<string> GetDelimitedListOfTwitchMemberIds(string teamToken)
        {
            return await twitchDal.GetDelimitedListOfTwitchMemberIds(teamToken);
        }

        public async Task<List<TwitchStreamsV5.Stream>> GetStreamsByGameName(string gameName)
        {
            return await twitchDal.GetStreamsByGameName(gameName);
        }

        public async Task<TwitchGameSearchResponse> SearchForGameByName(string gameName)
        {
            return await twitchDal.SearchForGameByName(gameName);
        }
    }
}
