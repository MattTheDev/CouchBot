using MTD.CouchBot.Domain.Dtos.Twitch;
using System.Threading.Tasks;

namespace MTD.CouchBot.Managers
{
    public interface ITwitchManager
    {
        Task<TwitchUserQueryResponse> GetTwitchUserByLoginName(string loginName);
    }
}
