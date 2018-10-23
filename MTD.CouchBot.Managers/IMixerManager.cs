using MTD.CouchBot.Domain.Dtos.Mixer;
using System.Threading.Tasks;

namespace MTD.CouchBot.Managers
{
    public interface IMixerManager
    {
        Task<MixerChannelQueryResponse> GetMixerChannelByChannelName(string channelName);
    }
}