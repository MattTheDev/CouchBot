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
    }
}
