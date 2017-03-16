using MTD.DiscordBot.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTD.DiscordBot.Dals
{
    public interface IBeamDal
    {
        Task<BeamChannel> GetBeamChannelByName(string name);
    }
}
