using MTD.CouchBot.Dals;
using MTD.CouchBot.Domain.Models.YouTube;
using System;
using System.Threading.Tasks;

namespace MTD.CouchBot.Managers
{
    public class YouTubeManager
    {
        private readonly IYouTubeDal _youtubeDal;

        public YouTubeManager(IYouTubeDal youtubeDal)
        {
            _youtubeDal = youtubeDal;
        }

        public async Task<YouTubeChannelStatistics> GetChannelStatisticsById(string id)
        {
            return await _youtubeDal.GetChannelStatisticsById(id);
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

        public async Task<string> GetPreviewUrl(string videoId)
        {
            return await _youtubeDal.GetPreviewUrl(videoId);
        }

        public async Task<LiveChatStatusResponse> GetLiveChannels(string channelIds)
        {
            return await _youtubeDal.GetLiveChannels(channelIds);
        }
    }
}
