using MTD.CouchBot.Domain.Models.Mixer;
using System.Threading.Tasks;

namespace MTD.CouchBot.Managers
{
    public interface IMixerManager
    {
        Task<MixerChannel> GetChannelByName(string name);
        Task<MixerChannel> GetChannelById(string id);
    }
}
