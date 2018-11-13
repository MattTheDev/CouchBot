using MTD.CouchBot.Domain.Dtos.Twitch;
using System.Threading.Tasks;

namespace MTD.CouchBot.Dals
{
    public interface ITwitchDal
    {
        Task<TwitchUserQueryResponse> GetTwitchUserByLoginName(string loginName);
        Task<TwitchUserQueryResponse> GetTwitchUsersByLoginNameDelimitedList(string loginNames);
        Task<TwitchUserQueryResponse> GetTwitchUserById(string id);
        Task<TwitchUserQueryResponse> GetTwitchUsersByIdsDelimitedList(string ids);
        Task<TwitchStreamQueryResponse> GetTwitchStreamByUserId(string id);
        Task<TwitchStreamQueryResponse> GetTwitchStreamsByUserIdsDelimitedList(string ids);
        Task<TwitchGameQueryResponse> GetTwitchGamesByIdsDelimitedList(string ids);
    }
}
