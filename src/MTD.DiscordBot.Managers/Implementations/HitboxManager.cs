using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MTD.DiscordBot.Domain.Models;
using MTD.DiscordBot.Dals;
using MTD.DiscordBot.Dals.Implementations;

namespace MTD.DiscordBot.Managers.Implementations
{
    public class HitboxManager : IHitboxManager
    {
        IHitboxDal hitboxDal;

        public HitboxManager()
        {
            hitboxDal = new HitboxDal();
        }

        public async Task<HitboxChannel> GetChannelByName(string name)
        {
            return await hitboxDal.GetChannelByName(name);
        }
    }
}
