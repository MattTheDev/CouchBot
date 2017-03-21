using MTD.CouchBot.Dals;
using MTD.CouchBot.Dals.Implementations;
using MTD.CouchBot.Domain.Models;
using System.Threading.Tasks;

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
