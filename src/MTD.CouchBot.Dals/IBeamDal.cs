using MTD.CouchBot.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTD.CouchBot.Dals
{
    public interface IBeamDal
    {
        Task<BeamChannel> GetBeamChannelByName(string name);
    }
}
