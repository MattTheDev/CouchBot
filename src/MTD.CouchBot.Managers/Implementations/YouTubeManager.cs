using MTD.CouchBot.Dals;
using MTD.CouchBot.Dals.Implementations;
using MTD.CouchBot.Domain.Models.YouTube;
using System.Threading.Tasks;

namespace MTD.CouchBot.Managers.Implementations
{
    public class YouTubeManager : IYouTubeManager
    {
        IYouTubeDal _youtubeDal;

        public YouTubeManager()
        {
            _youtubeDal = new YouTubeDal();
        }

        public async Task<YouTubeChannelStatistics> GetChannelStatisticsById(string id)
        {
            return await _youtubeDal.GetChannelStatisticsById(id);
        }

        public async Task<YouTubeChannelUpcomingEvents> GetChannelUpcomingEventsAsync(string channelId)
        {
            return await _youtubeDal.GetChannelUpcomingEvents(channelId);
        }

        public async Task<YouTubeChannelContentDetails> GetContentDetailsByChannelId(string channelId)
        {
            return await _youtubeDal.GetContentDetailsByChannelId(channelId);
        }

        public async Task<YouTubeSearchListChannel> GetLiveVideoByChannelId(string id)
        {
            return await _youtubeDal.GetLiveVideoByChannelId(id);
        }

        public async Task<YouTubePlaylist> GetPlaylistItemsByPlaylistId(string playlistId)
        {
            return await _youtubeDal.GetPlaylistItemsByPlaylistId(playlistId);
        }

        public async Task<YouTubeSearchListChannel> GetVideoById(string id)
        {
            return await _youtubeDal.GetVideoById(id);
        }

        public async Task<YouTubeChannelSearchList> GetYouTubeChannelByQuery(string name)
        {
            return await _youtubeDal.GetYouTubeChannelByQuery(name);
        }

        public async Task<YouTubeChannelSnippet> GetYouTubeChannelSnippetById(string channelId)
        {
            return await _youtubeDal.GetYouTubeChannelSnippetById(channelId);
        }
    }
}
