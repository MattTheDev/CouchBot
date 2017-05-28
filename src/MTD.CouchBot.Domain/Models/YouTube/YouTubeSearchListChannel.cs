using System.Collections.Generic;

namespace MTD.CouchBot.Domain.Models.YouTube
{
    public class YouTubeSearchListChannel
    {
        public string kind { get; set; }

        public string etag { get; set; }

        public PageInfo pageInfo { get; set; }

        public List<Item> items { get; set; }

        public class PageInfo
        {
            public int totalResults { get; set; }

            public int resultsPerPage { get; set; }
        }

        public class Default
        {
            public string url { get; set; }

            public int width { get; set; }

            public int height { get; set; }
        }

        public class Medium
        {
            public string url { get; set; }

            public int width { get; set; }

            public int height { get; set; }
        }

        public class High
        {
            public string url { get; set; }

            public int width { get; set; }

            public int height { get; set; }
        }

        public class Standard
        {
            public string url { get; set; }

            public int width { get; set; }

            public int height { get; set; }
        }

        public class Maxres
        {
            public string url { get; set; }

            public int width { get; set; }

            public int height { get; set; }
        }

        public class Thumbnails
        {
            public Default Default { get; set; }

            public Medium medium { get; set; }

            public High high { get; set; }

            public Standard standard { get; set; }

            public Maxres maxres { get; set; }
        }

        public class Localized
        {
            public string title { get; set; }

            public string description { get; set; }
        }

        public class Snippet
        {
            public string publishedAt { get; set; }
            public string channelId { get; set; }
            public string title { get; set; }
            public string description { get; set; }
            public Thumbnails thumbnails { get; set; }
            public string channelTitle { get; set; }
            public List<string> tags { get; set; }
            public string categoryId { get; set; }
            public string liveBroadcastContent { get; set; }
            public Localized localized { get; set; }
            public string defaultAudioLanguage { get; set; }
        }

        public class Statistics
        {
            public string viewCount { get; set; }
            public string likeCount { get; set; }
            public string dislikeCount { get; set; }
            public string favoriteCount { get; set; }
            public string commentCount { get; set; }
        }

        public class LiveStreamingDetails
        {
            public string actualStartTime { get; set; }
            public string scheduledStartTime { get; set; }
            public string concurrentViewers { get; set; }
            public string activeLiveChatId { get; set; }
        }

        public class Item
        {
            public string kind { get; set; }
            public string etag { get; set; }
            public string id { get; set; }
            public Snippet snippet { get; set; }
            public Statistics statistics { get; set; }
            public LiveStreamingDetails liveStreamingDetails { get; set; }
        }
    }
}
