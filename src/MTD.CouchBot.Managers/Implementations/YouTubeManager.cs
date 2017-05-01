using MTD.CouchBot.Dals;
using MTD.CouchBot.Dals.Implementations;
using MTD.CouchBot.Domain.Models;
using System.Threading.Tasks;

namespace MTD.CouchBot.Managers.Implementations
{
    public class YouTubeManager : IYouTubeManager
    {
        IYouTubeDal youtubeDal;

        public YouTubeManager()
        {
            youtubeDal = new YouTubDal();
        }

        public async Task<YouTubeChannelStatistics> GetChannelStatisticsById(string id)
        {
            return await youtubeDal.GetChannelStatisticsById(id);
        }

        public async Task<YouTubeChannelContentDetails> GetContentDetailsByChannelId(string channelId)
        {
            return await youtubeDal.GetContentDetailsByChannelId(channelId);
        }

        public async Task<YouTubeSearchListChannel> GetLiveVideoByChannelId(string id)
        {
            return await youtubeDal.GetLiveVideoByChannelId(id);
        }

        public async Task<YouTubePlaylist> GetPlaylistItemsByPlaylistId(string playlistId)
        {
            return await youtubeDal.GetPlaylistItemsByPlaylistId(playlistId);
        }

        public async Task<YouTubeSearchListChannel> GetVideoById(string id)
        {
            return await youtubeDal.GetVideoById(id);
        }

        public async Task<YouTubeChannelSearchList> GetYouTubeChannelByQuery(string name)
        {
            return await youtubeDal.GetYouTubeChannelByQuery(name);
        }

        public async Task<YouTubeChannelSnippet> GetYouTubeChannelSnippetById(string channelId)
        {
            return await youtubeDal.GetYouTubeChannelSnippetById(channelId);
        }
    }
}
