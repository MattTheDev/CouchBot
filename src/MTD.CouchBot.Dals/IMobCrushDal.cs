using MTD.CouchBot.Domain.Models.Mobcrush;
using System.Threading.Tasks;

namespace MTD.CouchBot.Dals
{
    public interface IMobcrushDal
    {
        Task<ChannelResponse> GetMobcrushIdByName(string name);
        Task<UserResponse> GetMobcrushStreamById(string id);
        Task<ChannelBroadcastResponse> GetMobcrushBroadcastByChannelId(string id);
        Task<ChannelByIdResponse> GetMobcrushChannelById(string id);
    }
}
