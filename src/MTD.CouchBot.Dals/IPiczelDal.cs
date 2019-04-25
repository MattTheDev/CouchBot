using MTD.CouchBot.Domain.Models.Piczel;
using System.Threading.Tasks;

namespace MTD.CouchBot.Dals
{
    public interface IPiczelDal
    {
        Task<int?> GetUserIdByName(string name);
        Task<PiczelStreamResponse> GetStreamById(int id);
    }
}
