using MTD.CouchBot.Domain.Models;
using System.Threading.Tasks;

namespace MTD.CouchBot.Dals
{
    public interface IMixerDal
    {
        Task<BeamChannel> GetChannelByName(string name);
        Task<BeamChannel> GetChannelById(string id);
    }
}
