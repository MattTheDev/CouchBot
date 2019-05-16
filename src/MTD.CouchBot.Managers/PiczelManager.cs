using System.Threading.Tasks;
using MTD.CouchBot.Dals;
using MTD.CouchBot.Domain.Models.Piczel;

namespace MTD.CouchBot.Managers
{
    public class PiczelManager
    {
        private readonly IPiczelDal _piczelDal;

        public PiczelManager(IPiczelDal piczelDal)
        {
            _piczelDal = piczelDal;
        }

        public async Task<PiczelStreamResponse> GetStreamById(int id)
        {
            return await _piczelDal.GetStreamById(id);
        }

        public async Task<int?> GetUserIdByName(string name)
        {
            return await _piczelDal.GetUserIdByName(name);
        }
    }
}
