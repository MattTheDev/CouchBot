using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MTD.CouchBot.Domain.Models;
using MTD.CouchBot.Dals;
using MTD.CouchBot.Dals.Implementations;

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

    }
}
