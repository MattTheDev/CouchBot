using MTD.CouchBot.Domain.Dtos.Twitch;
using System.Threading.Tasks;
using MTD.CouchBot.Domain.Dtos.YouTube;

namespace MTD.CouchBot.Dals
{
    public interface IYouTubeDal
    {
        Task<YouTubeChannelQueryByChannelIdResponse> GetYouTubeChannelByChannelId(string channelId);
    }
}
