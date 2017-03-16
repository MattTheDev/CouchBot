using MTD.DiscordBot.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTD.DiscordBot.Managers
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
