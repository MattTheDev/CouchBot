using MTD.CouchBot.Domain.Dtos.Twitch;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MTD.CouchBot.Managers
{
    public interface ITwitchManager
    {
        Task<TwitchUserQueryResponse> GetTwitchUserByLoginName(string loginName);
    }
}
