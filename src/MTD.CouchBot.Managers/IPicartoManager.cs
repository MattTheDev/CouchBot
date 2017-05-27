using MTD.CouchBot.Domain.Models.Picarto;
using System.Threading.Tasks;

namespace MTD.CouchBot.Managers
{
    public interface IPicartoManager
    {
        Task<PicartoChannel> GetChannelByName(string name);
    }
}
