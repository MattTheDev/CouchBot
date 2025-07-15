using System.Net;
using CB.Accessors.Contracts;
using CB.Shared.Models.YouTube;
using CB.Shared.Utilities;

namespace CB.Accessors.Implementations;

public class YouTubeAccessor : IYouTubeAccessor
{
    private readonly List<(string name, string value)> _youtubeHeaders = [("Content-Type", "application/json; charset=utf-8")];

    private readonly string _youTubeApiKey;
    private readonly IHttpClientFactory _httpClientFactory;

    public YouTubeAccessor(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _youTubeApiKey = Environment.GetEnvironmentVariable("YouTubeApiKey");
        if (string.IsNullOrEmpty(_youTubeApiKey))
        {
            throw new InvalidOperationException("YouTubeApiKey Configuration Missing.");
        }
    }

    public async Task<YouTubeChannelStatistics> GetChannelStatisticsByIdAsync(string id)
    {
        return await ApiUtilities
            .ApiHelper<YouTubeChannelStatistics>($"https://www.googleapis.com/youtube/v3/channels?part=statistics&key={_youTubeApiKey}&id={id}", 
                _youtubeHeaders)
            .ConfigureAwait(false);
    }

    public async Task<YouTubeSearchListChannel> GetVideoByIdAsync(string id)
    {
        return await ApiUtilities
            .ApiHelper<YouTubeSearchListChannel>($"https://www.googleapis.com/youtube/v3/videos?part=snippet,statistics,liveStreamingDetails&key={_youTubeApiKey}&id={id}", 
                _youtubeHeaders)
            .ConfigureAwait(false);
    }

    public async Task<YouTubePlaylist> GetPlaylistItemsByPlaylistIdAsync(string playlistId)
    {
        using var client = _httpClientFactory.CreateClient();
        return await ApiUtilities
            .ApiHelper<YouTubePlaylist>(client, 
                $"https://www.googleapis.com/youtube/v3/playlistItems?part=snippet,contentDetails,status&maxResults=5&key={_youTubeApiKey}&playlistId={playlistId}", 
                _youtubeHeaders)
            .ConfigureAwait(false);
    }

    public async Task<YouTubeChannelSearchList> GetYouTubeChannelByQueryAsync(string name)
    {
        return await ApiUtilities
            .ApiHelper<YouTubeChannelSearchList>($"https://www.googleapis.com/youtube/v3/search?part=snippet&q={name}&type=channel&key={_youTubeApiKey}", 
                _youtubeHeaders)
            .ConfigureAwait(false);
    }

    public async Task<YouTubeChannelSearchList> GetYouTubeChannelByUsernameAsync(string username)
    {
        return await ApiUtilities
            .ApiHelper<YouTubeChannelSearchList>($"https://www.googleapis.com/youtube/v3/channels?part=snippet,contentDetails&forUsername={username}&type=channel&key={_youTubeApiKey}", 
                _youtubeHeaders)
            .ConfigureAwait(false);
    }

    public async Task<YouTubeChannelByIdResponse> GetYouTubeChannelByIdAsync(string channelId)
    {
        using var client = _httpClientFactory.CreateClient();
        return await ApiUtilities
            .ApiHelper<YouTubeChannelByIdResponse>(client, 
                $"https://www.googleapis.com/youtube/v3/channels?part=snippet,contentDetails&id={channelId}&key={_youTubeApiKey}", 
                _youtubeHeaders)
            .ConfigureAwait(false);
    }

    public async Task<LiveChatStatusResponse> GetLiveChannelsAsync(string channelIds)
    {
        return await ApiUtilities
            .ApiHelper<LiveChatStatusResponse>("https://www.googleapis.com/youtube/v3/liveChat/status?" +
                                                                    $"channelId={channelIds}&part=snippet&key={_youTubeApiKey}&" +
                                                                    "fields=items(snippet(channelId,currentVideoId))", 
                _youtubeHeaders)
            .ConfigureAwait(false);
    }

    public async Task<YouTubeVideoListResponse> GetVideoDetailsAsync(string videoId)
    {
        using var client = _httpClientFactory.CreateClient();
        return await ApiUtilities
            .ApiHelper<YouTubeVideoListResponse>(client, 
            $"https://www.googleapis.com/youtube/v3/videos?part=snippet,contentDetails,status,liveStreamingDetails&key={_youTubeApiKey}&id={videoId}", 
            _youtubeHeaders)
            .ConfigureAwait(false);
    }

    public async Task<List<YouTubeChannelByIdResponse>> GetYouTubeChannelsByIdsAsync(string channelIds)
    {
        return await ApiUtilities
            .ApiHelper<List<YouTubeChannelByIdResponse>>($"https://www.googleapis.com/youtube/v3/channels?part=snippet,contentDetails&id={channelIds}&key={_youTubeApiKey}", 
                _youtubeHeaders)
            .ConfigureAwait(false);
    }

    public async Task<bool> IsYouTubeShortAsync(string videoId)
    {
        var url = $"https://www.youtube.com/shorts/{videoId}";

        using var client = new HttpClient(new HttpClientHandler { AllowAutoRedirect = false });

        return (await client
            .GetAsync(url)
            .ConfigureAwait(false)).StatusCode == HttpStatusCode.OK;
    }
}