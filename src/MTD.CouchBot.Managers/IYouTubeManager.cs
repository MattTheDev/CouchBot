using MTD.CouchBot.Domain.Models;
using System.Threading.Tasks;

namespace MTD.CouchBot.Managers
{
    public interface IYouTubeManager
    {
        Task<YouTubeSearchListChannel> GetLiveVideoByChannelId(string id);
        Task<YouTubeSearchListChannel> GetVideoById(string id);
        Task<YouTubeChannelStatistics> GetChannelStatisticsById(string id);
        Task<YouTubePlaylist> GetPlaylistItemsByPlaylistId(string playlistId);
        Task<YouTubeChannelContentDetails> GetContentDetailsByChannelId(string channelId);
        Task<YouTubeChannelSearchList> GetYouTubeChannelByQuery(string name);
        Task<YouTubeChannelSnippet> GetYouTubeChannelSnippetById(string channelId);
    }
}
