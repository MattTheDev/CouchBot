using System.Threading.Tasks;
using MTD.CouchBot.Domain.Dtos.Mixer;

namespace MTD.CouchBot.Managers
{
    public interface IMixerManager
    {
        Task<MixerChannelQueryResponse> GetMixerChannelByChannelName(string channelName);
    }
}