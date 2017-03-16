using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTD.DiscordBot.Domain.Models
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
