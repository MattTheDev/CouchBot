using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MTD.CouchBot.Domain.Models;
using MTD.CouchBot.Dals;
using MTD.CouchBot.Dals.Implementations;
using Discord;
using MTD.CouchBot.Domain.Utilities;
using MTD.CouchBot.Models;
using System.IO;
using MTD.CouchBot.Domain;
using Newtonsoft.Json;

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
