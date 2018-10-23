using MTD.CouchBot.Dals;
using MTD.CouchBot.Domain.Dtos.YouTube;
using System.Threading.Tasks;

namespace MTD.CouchBot.Managers.Implementations
{
    public class YouTubeManager : IYouTubeManager
    {
        private readonly IYouTubeDal _youTubeDal;

        public YouTubeManager(IYouTubeDal youTubeDal)
        {
            _youTubeDal = youTubeDal;
        }

        public async Task<YouTubeChannelQueryByChannelIdResponse> GetYouTubeChannelByChannelId(string channelId)
        {
            return await _youTubeDal.GetYouTubeChannelByChannelId(channelId);
        }
    }
}
