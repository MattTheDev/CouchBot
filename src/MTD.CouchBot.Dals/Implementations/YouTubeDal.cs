using Microsoft.Extensions.Options;
using MTD.CouchBot.Domain.Models.Bot;
using MTD.CouchBot.Domain.Models.YouTube;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace MTD.CouchBot.Dals.Implementations
{
    public class YouTubeDal : IYouTubeDal
    {
        private readonly BotSettings _botSettings;

        public YouTubeDal(IOptions<BotSettings> botSettings)
        {
            _botSettings = botSettings.Value;
        }

        public async Task<YouTubeChannelStatistics> GetChannelStatisticsById(string id)
        {
            try
            {
                var webRequest = (HttpWebRequest)WebRequest.Create("https://www.googleapis.com/youtube/v3/channels?part=statistics&key=" + _botSettings.KeySettings.YouTubeApiKey + "&id=" + id);
                webRequest.ContentType = "application/json; charset=utf-8";
                string str;
                using (var streamReader = new StreamReader((await webRequest.GetResponseAsync()).GetResponseStream()))
                    str = streamReader.ReadToEnd();
                return JsonConvert.DeserializeObject<YouTubeChannelStatistics>(str);
            }
            catch(Exception)
            {
                return null;
            }
        }

        public async Task<YouTubeSearchListChannel> GetLiveVideoByChannelId(string id)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create("https://www.googleapis.com/youtube/v3/search?part=snippet&channelId=" + id + "&eventType=live&maxResults=1&type=video&key=" + _botSettings.KeySettings.YouTubeApiKey + "&fields=items(id/videoId)");
            webRequest.ContentType = "application/json; charset=utf-8";
            var str = "";

            using (var streamReader = new StreamReader((await webRequest.GetResponseAsync()).GetResponseStream()))
                str = streamReader.ReadToEnd();

            if (!string.IsNullOrEmpty(str))
            {
                var channel = JsonConvert.DeserializeObject<SearchLiveEvent>(str);
                if (channel.items.Count > 0)
                {
                    return await GetVideoById(channel.items[0].id.videoId).ConfigureAwait(false);
                }
                else
                {
                    return null;
                }
            }

            return null;
        }

        public async Task<YouTubeSearchListChannel> GetVideoById(string id)
        {
            try
            {
                var webRequest = (HttpWebRequest)WebRequest.Create("https://www.googleapis.com/youtube/v3/videos?part=snippet,statistics,liveStreamingDetails&key=" + _botSettings.KeySettings.YouTubeApiKey + "&id=" + id);
                webRequest.ContentType = "application/json; charset=utf-8";
                string str;
                using (var streamReader = new StreamReader((await webRequest.GetResponseAsync()).GetResponseStream()))
                    str = streamReader.ReadToEnd();
                return JsonConvert.DeserializeObject<YouTubeSearchListChannel>(str);
            }
            catch(Exception)
            {
                // TODO MS
                //Logging.LogError("Error in GetVideoById: " + ex.Message); 
            }

            return null;
        }

        public async Task<YouTubeChannelContentDetails> GetContentDetailsByChannelId(string channelId)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create("https://www.googleapis.com/youtube/v3/channels?part=contentDetails&key=" + _botSettings.KeySettings.YouTubeApiKey + "&id=" + channelId);
            webRequest.ContentType = "application/json; charset=utf-8";
            string str;
            using (var streamReader = new StreamReader((await webRequest.GetResponseAsync()).GetResponseStream()))
                str = streamReader.ReadToEnd();
            return JsonConvert.DeserializeObject<YouTubeChannelContentDetails>(str);
        }

        public async Task<YouTubePlaylist> GetPlaylistItemsByPlaylistId(string playlistId)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create("https://www.googleapis.com/youtube/v3/playlistItems?part=snippet&maxResults=25&key=" + _botSettings.KeySettings.YouTubeApiKey + "&playlistId=" + playlistId);
            webRequest.ContentType = "application/json; charset=utf-8";
            string str;
            using (var streamReader = new StreamReader((await webRequest.GetResponseAsync()).GetResponseStream()))
                str = streamReader.ReadToEnd();

            return JsonConvert.DeserializeObject<YouTubePlaylist>(str);
        }

        public async Task<YouTubeChannelSearchList> GetYouTubeChannelByQuery(string name)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create("https://www.googleapis.com/youtube/v3/search?part=snippet&q=" + name + "&type=channel&key=" + _botSettings.KeySettings.YouTubeApiKey);
            webRequest.ContentType = "application/json; charset=utf-8";
            var str = "";

            using (var streamReader = new StreamReader((await webRequest.GetResponseAsync()).GetResponseStream()))
                str = streamReader.ReadToEnd();

            return JsonConvert.DeserializeObject<YouTubeChannelSearchList>(str);
        }

        public async Task<YouTubeChannelSnippet> GetYouTubeChannelSnippetById(string channelId)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create("https://www.googleapis.com/youtube/v3/channels?part=snippet&id=" + channelId + "&key=" + _botSettings.KeySettings.YouTubeApiKey);
            webRequest.ContentType = "application/json; charset=utf-8";
            var str = "";

            using (var streamReader = new StreamReader((await webRequest.GetResponseAsync()).GetResponseStream()))
                str = streamReader.ReadToEnd();

            return JsonConvert.DeserializeObject<YouTubeChannelSnippet>(str);
        }

        public async Task<string> GetPreviewUrl(string videoId)
        {
            var url = "https://i.ytimg.com/an_webp/" + videoId + "/mqdefault_6s.webp";//?du=3000&amp;sqp=CLyroMsF&amp;rs=";
            var webRequest = (HttpWebRequest)WebRequest.Create(
                "https://www.youtube.com/results?search_query=" + videoId);
            webRequest.Headers[HttpRequestHeader.UserAgent] = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.116 Safari/537.36";

            var str = "";

            using (var streamReader = new StreamReader((await webRequest.GetResponseAsync()).GetResponseStream()))
                str = streamReader.ReadToEnd();

            var start = str.IndexOf(url);
            str = str.Substring(start);
            str = str.Replace(url, "");
            start = str.IndexOf('"');
            str = str.Substring(0, start);

            return (url + str).Replace("&amp;", "&");
        }

        public async Task<LiveChatStatusResponse> GetLiveChannels(string channelIds)
        {
            try
            {
                var channelList = string.Join(",", channelIds);
                var webRequest = (HttpWebRequest)WebRequest.Create(
                    $"https://www.googleapis.com/youtube/v3/liveChat/status?" +
                    $"channelId={channelIds}&part=snippet&key={_botSettings.KeySettings.YouTubeApiKey}&" +
                    $"fields=items(snippet(channelId,currentVideoId))");

                string str;
                using (var streamReader = new StreamReader((await webRequest.GetResponseAsync()).GetResponseStream()))
                    str = streamReader.ReadToEnd();

                return JsonConvert.DeserializeObject<LiveChatStatusResponse>(str);
            }
            catch(Exception) { }

            return null;
        }
    }
}
