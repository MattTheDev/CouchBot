using MTD.CouchBot.Domain.Dtos.Twitch;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MTD.CouchBot.Managers
{
    public interface ITwitchManager
    {
        Task<TwitchUserQueryResponse> GetTwitchUserByLoginName(string loginName);
        Task<TwitchUserQueryResponse> GetTwitchUsersByLoginNameDelimitedList(string loginNames);
        Task<TwitchUserQueryResponse> GetTwitchUserById(string id);
        Task<TwitchUserQueryResponse> GetTwitchUsersByIdsDelimitedList(string ids);
        Task<TwitchStreamQueryResponse> GetTwitchStreamByUserId(string id);
        Task<TwitchStreamQueryResponse> GetTwitchStreamsByUserIdsDelimitedList(List<string> ids);
        Task<TwitchGameQueryResponse> GetTwitchGamesByIdsDelimitedList(List<string> ids);
    }
}
