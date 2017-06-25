using MTD.CouchBot.Dals;
using MTD.CouchBot.Dals.Implementations;
using MTD.CouchBot.Domain.Models.Mixer;
using System.Threading.Tasks;

namespace MTD.CouchBot.Managers.Implementations
{
    public class MixerManager : IMixerManager
    {
        readonly IMixerDal _mixerDal;

        public MixerManager()
        {
            _mixerDal = new MixerDal();
        }

        public async Task<MixerChannel> GetChannelById(string id)
        {
            return await _mixerDal.GetChannelById(id);
        }

        public async Task<MixerChannel> GetChannelByName(string name)
        {
            return await _mixerDal.GetChannelByName(name);
        }
    }
}
