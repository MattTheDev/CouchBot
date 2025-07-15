using CB.Shared.Models.YouTube;

namespace CB.Accessors.Contracts;

public interface IYouTubeAccessor
{
    Task<YouTubeSearchListChannel> GetVideoByIdAsync(string id);

    Task<YouTubeChannelStatistics> GetChannelStatisticsByIdAsync(string id);

    Task<YouTubePlaylist> GetPlaylistItemsByPlaylistIdAsync(string playlistId);

    Task<YouTubeChannelSearchList> GetYouTubeChannelByQueryAsync(string name);

    Task<YouTubeChannelByIdResponse> GetYouTubeChannelByIdAsync(string channelId);

    Task<LiveChatStatusResponse> GetLiveChannelsAsync(string channelIds);

    Task<YouTubeVideoListResponse> GetVideoDetailsAsync(string videoId);

    Task<List<YouTubeChannelByIdResponse>> GetYouTubeChannelsByIdsAsync(string channelIds);

    Task<YouTubeChannelSearchList> GetYouTubeChannelByUsernameAsync(string username);

    Task<bool> IsYouTubeShortAsync(string videoId);
}