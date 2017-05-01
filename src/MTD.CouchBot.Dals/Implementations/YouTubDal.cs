using MTD.CouchBot.Domain;
using MTD.CouchBot.Domain.Models;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace MTD.CouchBot.Dals.Implementations
{
    public class YouTubDal : IYouTubeDal
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
                    return await GetVideoById(channel.items[0].id.videoId);
                else
                    return null;
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
            var webRequest = (HttpWebRequest)WebRequest.Create("https://www.googleapis.com/youtube/v3/playlistItems?part=snippet&maxResults=10&key=" + Constants.YouTubeApiKey + "&playlistId=" + playlistId);
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
    }
}
