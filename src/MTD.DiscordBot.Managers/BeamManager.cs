using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MTD.DiscordBot.Domain.Models;
using MTD.DiscordBot.Dals;
using MTD.DiscordBot.Dals.Implementations;

namespace MTD.DiscordBot.Managers
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
