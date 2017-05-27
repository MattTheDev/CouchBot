using MTD.CouchBot.Dals;
using MTD.CouchBot.Dals.Implementations;
using MTD.CouchBot.Domain.Models.Picarto;
using System.Threading.Tasks;

namespace MTD.CouchBot.Managers.Implementations
{
    public class PicartoManager : IPicartoManager
    {
        IPicartoDal _picartoDal;

        public PicartoManager()
        {
            _picartoDal = new PicartoDal();
        }

        public async Task<PicartoChannel> GetChannelByName(string name)
        {
            return await _picartoDal.GetChannelByName(name);
        }
    }
}
