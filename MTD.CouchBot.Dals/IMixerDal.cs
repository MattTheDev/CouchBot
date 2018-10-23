using MTD.CouchBot.Domain.Dtos.Mixer;
using System.Threading.Tasks;

namespace MTD.CouchBot.Dals
{
    public interface IMixerDal
    {
        Task<MixerChannelQueryResponse> GetMixerChannelByChannelName(string channelName);
    }
}
