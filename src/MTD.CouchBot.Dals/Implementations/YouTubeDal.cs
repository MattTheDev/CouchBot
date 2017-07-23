using MTD.CouchBot.Domain;
using MTD.CouchBot.Domain.Models.YouTube;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System;

namespace MTD.CouchBot.Dals.Implementations
{
    public class YouTubeDal : IYouTubeDal
    {
        public async Task<YouTubeChannelStatistics> GetChannelStatisticsById(string id)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create("https://www.googleapis.com/youtube/v3/channels?part=statistics&key=" + Constants.YouTubeApiKey + " &id=" + id);
            webRequest.ContentType = "application/json; charset=utf-8";
            string str;
            using (StreamReader streamReader = new StreamReader((await webRequest.GetResponseAsync()).GetResponseStream()))
                str = streamReader.ReadToEnd();
            return JsonConvert.DeserializeObject<YouTubeChannelStatistics>(str);
        }

        public async Task<YouTubeSearchListChannel> GetLiveVideoByChannelId(string id)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create("https://www.googleapis.com/youtube/v3/search?part=snippet&channelId=" + id + "&eventType=live&maxResults=1&type=video&key=" + Constants.YouTubeApiKey + "&fields=items(id/videoId)");
            webRequest.ContentType = "application/json; charset=utf-8";
            string str = "";

            using (StreamReader streamReader = new StreamReader((await webRequest.GetResponseAsync()).GetResponseStream()))
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
            var webRequest = (HttpWebRequest)WebRequest.Create("https://www.googleapis.com/youtube/v3/videos?part=snippet,statistics,liveStreamingDetails&key=" + Constants.YouTubeApiKey + "&id=" + id);
            webRequest.ContentType = "application/json; charset=utf-8";
            string str;
            using (StreamReader streamReader = new StreamReader((await webRequest.GetResponseAsync()).GetResponseStream()))
                str = streamReader.ReadToEnd();
            return JsonConvert.DeserializeObject<YouTubeSearchListChannel>(str);
        }

        public async Task<YouTubeChannelContentDetails> GetContentDetailsByChannelId(string channelId)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create("https://www.googleapis.com/youtube/v3/channels?part=contentDetails&key=" + Constants.YouTubeApiKey + "&id=" + channelId);
            webRequest.ContentType = "application/json; charset=utf-8";
            string str;
            using (StreamReader streamReader = new StreamReader((await webRequest.GetResponseAsync()).GetResponseStream()))
                str = streamReader.ReadToEnd();
            return JsonConvert.DeserializeObject<YouTubeChannelContentDetails>(str);
        }

        public async Task<YouTubePlaylist> GetPlaylistItemsByPlaylistId(string playlistId)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create("https://www.googleapis.com/youtube/v3/playlistItems?part=snippet&maxResults=25&key=" + Constants.YouTubeApiKey + "&playlistId=" + playlistId);
            webRequest.ContentType = "application/json; charset=utf-8";
            string str;
            using (StreamReader streamReader = new StreamReader((await webRequest.GetResponseAsync()).GetResponseStream()))
                str = streamReader.ReadToEnd();

            return JsonConvert.DeserializeObject<YouTubePlaylist>(str);
        }

        public async Task<YouTubeChannelSearchList> GetYouTubeChannelByQuery(string name)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create("https://www.googleapis.com/youtube/v3/search?part=snippet&q=" + name + "&type=channel&key=" + Constants.YouTubeApiKey);
            webRequest.ContentType = "application/json; charset=utf-8";
            string str = "";

            using (StreamReader streamReader = new StreamReader((await webRequest.GetResponseAsync()).GetResponseStream()))
                str = streamReader.ReadToEnd();

            return JsonConvert.DeserializeObject<YouTubeChannelSearchList>(str);
        }

        public async Task<YouTubeChannelSnippet> GetYouTubeChannelSnippetById(string channelId)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create("https://www.googleapis.com/youtube/v3/channels?part=snippet&id=" + channelId + "&key=" + Constants.YouTubeApiKey);
            webRequest.ContentType = "application/json; charset=utf-8";
            string str = "";

            using (StreamReader streamReader = new StreamReader((await webRequest.GetResponseAsync()).GetResponseStream()))
                str = streamReader.ReadToEnd();

            return JsonConvert.DeserializeObject<YouTubeChannelSnippet>(str);
        }

        public async Task<YouTubeChannelUpcomingEvents> GetChannelUpcomingEvents(string channelId)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create(
                            "https://www.googleapis.com/youtube/v3/search?part=snippet&channelId=" + channelId +
                            "&type=video&eventType=upcoming&maxResults=25&key=" + Constants.YouTubeApiKey);
            webRequest.ContentType = "application/json; charset=utf-8";
            string str = "";

            using (StreamReader streamReader = new StreamReader((await webRequest.GetResponseAsync()).GetResponseStream()))
                str = streamReader.ReadToEnd();

            return JsonConvert.DeserializeObject<YouTubeChannelUpcomingEvents>(str);
        }

        public async Task<string> GetPreviewUrl(string videoId)
        {
            var url = "https://i.ytimg.com/an_webp/" + videoId + "/mqdefault_6s.webp";//?du=3000&amp;sqp=CLyroMsF&amp;rs=";
            var webRequest = (HttpWebRequest)WebRequest.Create(
                "https://www.youtube.com/results?search_query=" + videoId);
            webRequest.Headers[HttpRequestHeader.UserAgent] = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.116 Safari/537.36";

            string str = "";

            using (StreamReader streamReader = new StreamReader((await webRequest.GetResponseAsync()).GetResponseStream()))
                str = streamReader.ReadToEnd();

            var start = str.IndexOf(url);
            str = str.Substring(start);
            str = str.Replace(url, "");
            start = str.IndexOf('"');
            str = str.Substring(0, start);

            return (url + str).Replace("&amp;", "&");
        }
    }
}
