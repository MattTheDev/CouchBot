using System.Collections.Generic;
using MTD.CouchBot.Dals;
using MTD.CouchBot.Domain.Dtos.Twitch;
using System.Threading.Tasks;

namespace MTD.CouchBot.Managers.Implementations
{
    public class TwitchManager : ITwitchManager
    {
        private readonly ITwitchDal _twitchDal;

        public TwitchManager(ITwitchDal twitchDal)
        {
            _twitchDal = twitchDal;
        }

        public async Task<TwitchStreamQueryResponse> GetTwitchStreamByUserId(string id)
        {
            return await _twitchDal.GetTwitchStreamByUserId(id);
        }

        public async Task<TwitchStreamQueryResponse> GetTwitchStreamsByUserIdsDelimitedList(List<string> ids)
        {
            return await _twitchDal.GetTwitchStreamsByUserIdsDelimitedList($"&user_id={string.Join("&user_id=", ids)}");
        }

        public async Task<TwitchUserQueryResponse> GetTwitchUserById(string id)
        {
            return await _twitchDal.GetTwitchUserById(id);
        }

        public async Task<TwitchUserQueryResponse> GetTwitchUserByLoginName(string loginName)
        {
            return await _twitchDal.GetTwitchUserByLoginName(loginName);
        }

        public async Task<TwitchUserQueryResponse> GetTwitchUsersByIdsDelimitedList(string ids)
        {
            return await _twitchDal.GetTwitchUsersByIdsDelimitedList(ids);
        }

        public async Task<TwitchUserQueryResponse> GetTwitchUsersByLoginNameDelimitedList(string loginNames)
        {
            return await _twitchDal.GetTwitchUsersByLoginNameDelimitedList(loginNames);
        }
    }
}
