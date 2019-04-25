using MTD.CouchBot.Dals;
using MTD.CouchBot.Domain.Models.Mobcrush;
using System.Threading.Tasks;

namespace MTD.CouchBot.Managers.Implementations
{
    public class MobcrushManager : IMobcrushManager
    {
        private readonly IMobcrushDal _mobcrushDal;

        public MobcrushManager(IMobcrushDal mobcrushDal)
        {
            _mobcrushDal = mobcrushDal;
        }

        public async Task<ChannelBroadcastResponse> GetMobcrushBroadcastByChannelId(string id)
        {
            return await _mobcrushDal.GetMobcrushBroadcastByChannelId(id);
        }

        public async Task<ChannelByIdResponse> GetMobcrushChannelById(string id)
        {
            return await _mobcrushDal.GetMobcrushChannelById(id);
        }

        public async Task<ChannelResponse> GetMobcrushIdByName(string name)
        {
            return await _mobcrushDal.GetMobcrushIdByName(name);
        }

        public async Task<UserResponse> GetMobcrushStreamById(string id)
        {
            return await _mobcrushDal.GetMobcrushStreamById(id);
        }
    }
}
