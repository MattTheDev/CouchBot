using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MTD.CouchBot.Domain.Models;
using MTD.CouchBot.Dals;
using MTD.CouchBot.Dals.Implementations;

namespace MTD.CouchBot.Managers
{
    public class BeamManager : IBeamManager
    {
        IBeamDal beamDal;

        public BeamManager()
        {
            beamDal = new BeamDal();
        }

        public async Task<BeamChannel> GetBeamChannelByName(string name)
        {
            return await beamDal.GetBeamChannelByName(name);
        }
    }
}
