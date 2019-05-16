using MTD.CouchBot.Dals;
using MTD.CouchBot.Domain.Models.Picarto;
using System.Threading.Tasks;

namespace MTD.CouchBot.Managers.Implementations
{
    public class PicartoManager
    {
        private readonly IPicartoDal _picartoDal;

        public PicartoManager(IPicartoDal picartoDal)
        {
            _picartoDal = picartoDal;
        }

        public async Task<PicartoChannel> GetChannelByName(string name)
        {
            return await _picartoDal.GetChannelByName(name);
        }
    }
}
