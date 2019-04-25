using MTD.CouchBot.Domain.Models.Picarto;
using System.Threading.Tasks;

namespace MTD.CouchBot.Dals
{
    public interface IPicartoDal
    {
        Task<PicartoChannel> GetChannelByName(string name);
    }
}
