using MTD.CouchBot.Dals;
using MTD.CouchBot.Dals.Implementations;
using MTD.CouchBot.Domain.Models.Mixer;
using System.Threading.Tasks;

namespace MTD.CouchBot.Managers.Implementations
{
    public class MixerManager : IMixerManager
    {
        IMixerDal mixerDal;
        IStatisticsDal _statisticsDal;

        public MixerManager()
        {
            mixerDal = new MixerDal();
            _statisticsDal = new StatisticsDal();
        }

        public async Task<MixerChannel> GetChannelById(string id)
        {
            return await mixerDal.GetChannelById(id);
        }

        public async Task<MixerChannel> GetChannelByName(string name)
        {
            return await mixerDal.GetChannelByName(name);
        }
    }
}
