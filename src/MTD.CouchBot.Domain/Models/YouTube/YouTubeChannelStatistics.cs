using System.Collections.Generic;

namespace MTD.CouchBot.Domain.Models.YouTube
{
    public class YouTubeChannelStatistics
    {
        public class PageInfo
        {
            public int totalResults { get; set; }
            public int resultsPerPage { get; set; }
        }

        public class Statistics
        {
            public string viewCount { get; set; }
            public string commentCount { get; set; }
            public string subscriberCount { get; set; }
            public bool hiddenSubscriberCount { get; set; }
            public string videoCount { get; set; }
        }

        public class Item
        {
            public string kind { get; set; }
            public string etag { get; set; }
            public string id { get; set; }
            public Statistics statistics { get; set; }
        }

        public string kind { get; set; }
        public string etag { get; set; }
        public PageInfo pageInfo { get; set; }
        public List<Item> items { get; set; }

    }
}
