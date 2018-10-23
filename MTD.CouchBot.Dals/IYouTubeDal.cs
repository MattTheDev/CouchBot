using MTD.CouchBot.Domain.Dtos.YouTube;
using System.Threading.Tasks;

namespace MTD.CouchBot.Dals
{
    public interface IYouTubeDal
    {
        Task<YouTubeChannelQueryByChannelIdResponse> GetYouTubeChannelByChannelId(string channelId);
    }
}
