using MTD.CouchBot.Domain.Models.Piczel;
using System.Threading.Tasks;

namespace MTD.CouchBot.Managers
{
    public interface IPiczelManager
    {
        Task<int?> GetUserIdByName(string name);
        Task<PiczelStreamResponse> GetStreamById(int id);
    }
}
