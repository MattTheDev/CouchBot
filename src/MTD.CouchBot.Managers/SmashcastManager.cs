using System.Threading.Tasks;
using MTD.CouchBot.Dals;
using MTD.CouchBot.Domain.Models.Smashcast;

namespace MTD.CouchBot.Managers
{
    public class SmashcastManager
    {
        private readonly ISmashcastDal _smashcastDal;

        public SmashcastManager(ISmashcastDal smashcastDal)
        {
            _smashcastDal = smashcastDal;
        }

        public async Task<SmashcastChannel> GetChannelByName(string name)
        {
            return await _smashcastDal.GetChannelByName(name);
        }
    }
}
