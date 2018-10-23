using MTD.CouchBot.Domain.Dtos.YouTube;
using System.Threading.Tasks;

namespace MTD.CouchBot.Managers
{
    public interface IYouTubeManager
    {
        Task<YouTubeChannelQueryByChannelIdResponse> GetYouTubeChannelByChannelId(string channelId);
    }
}
