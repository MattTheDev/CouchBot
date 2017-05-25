using MTD.CouchBot.Domain.Models;
using System.Threading.Tasks;

namespace MTD.CouchBot.Managers
{
    public interface IMixerManager
    {
        Task<BeamChannel> GetChannelByName(string name);
        Task<BeamChannel> GetChannelById(string id);
    }
}
