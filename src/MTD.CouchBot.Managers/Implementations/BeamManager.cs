using MTD.CouchBot.Dals;
using MTD.CouchBot.Dals.Implementations;
using MTD.CouchBot.Domain.Models;
using System.Threading.Tasks;

namespace MTD.CouchBot.Managers.Implementations
{
    public class BeamManager : IBeamManager
    {
        IBeamDal beamDal;
        IStatisticsDal _statisticsDal;

        public BeamManager()
        {
            beamDal = new BeamDal();
            _statisticsDal = new StatisticsDal();
        }

        public async Task<BeamChannel> GetBeamChannelByName(string name)
        {
            return await beamDal.GetBeamChannelByName(name);
        }
    }
}
