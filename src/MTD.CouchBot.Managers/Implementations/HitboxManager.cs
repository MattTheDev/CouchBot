using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MTD.CouchBot.Domain.Models;
using MTD.CouchBot.Dals;
using MTD.CouchBot.Dals.Implementations;

namespace MTD.CouchBot.Managers.Implementations
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
