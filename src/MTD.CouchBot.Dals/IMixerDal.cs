using MTD.CouchBot.Domain.Models.Mixer;
using System.Threading.Tasks;

namespace MTD.CouchBot.Dals
{
    public interface IMixerDal
    {
        Task<MixerChannel> GetChannelByName(string name);
        Task<MixerChannel> GetChannelById(string id);
    }
}
