using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MTD.CouchBot.Dals;
using MTD.CouchBot.Domain.Dtos.Twitch;

namespace MTD.CouchBot.Managers.Implementations
{
    public class TwitchManager : ITwitchManager
    {
        private readonly ITwitchDal _twitchDal;

        public TwitchManager(ITwitchDal twitchDal)
        {
            _twitchDal = twitchDal;
        }

        public async Task<TwitchUserQueryResponse> GetTwitchUserByLoginName(string loginName)
        {
            return await _twitchDal.GetTwitchUserByLoginName(loginName);
        }
    }
}
