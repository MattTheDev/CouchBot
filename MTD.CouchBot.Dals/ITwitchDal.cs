using MTD.CouchBot.Domain.Dtos.Twitch;
using System.Threading.Tasks;

namespace MTD.CouchBot.Dals
{
    public interface ITwitchDal
    {
        Task<TwitchUserQueryResponse> GetTwitchUserByLoginName(string loginName);
    }
}
