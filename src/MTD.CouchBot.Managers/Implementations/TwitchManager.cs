using MTD.CouchBot.Dals;
using MTD.CouchBot.Dals.Implementations;
using MTD.CouchBot.Domain.Models.Twitch;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
            var builder = new StringBuilder();
            foreach (var id in twitchIdList)
            {
                builder.Append(id + ",");
            }

            return await twitchDal.GetStreamsByIdList(builder.ToString().TrimEnd(','));
        }

        public async Task<TwitchChannelFeed> GetChannelFeedPosts(string twitchId)
        {
            return await twitchDal.GetChannelFeedPosts(twitchId);
        }
    }
}
