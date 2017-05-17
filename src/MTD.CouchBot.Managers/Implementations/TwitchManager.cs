using MTD.CouchBot.Dals;
using MTD.CouchBot.Dals.Implementations;
using MTD.CouchBot.Domain.Models;
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

        public async Task<TwitchChannelFeed> GetChannelFeedPosts(string twitchId)
        {
            return await twitchDal.GetChannelFeedPosts(twitchId);
        }
    }
}
