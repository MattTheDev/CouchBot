using MTD.CouchBot.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTD.CouchBot.Managers
{
    public interface IBeamManager
    {
        Task<BeamChannel> GetBeamChannelByName(string name);
    }
}
