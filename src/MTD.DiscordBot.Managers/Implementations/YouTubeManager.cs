using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MTD.DiscordBot.Domain.Models;
using MTD.DiscordBot.Dals;
using MTD.DiscordBot.Dals.Implementations;

namespace MTD.DiscordBot.Managers.Implementations
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
