using System.Collections.Generic;

namespace MTD.CouchBot.Domain.Models.YouTube
{
    public class YouTubeChannelContentDetails
    {
        public class PageInfo
        {
            public int totalResults { get; set; }
            public int resultsPerPage { get; set; }
        }

        public class RelatedPlaylists
        {
            public string likes { get; set; }
            public string uploads { get; set; }
            public string watchHistory { get; set; }
            public string watchLater { get; set; }
        }

        public class ContentDetails
        {
            public RelatedPlaylists relatedPlaylists { get; set; }
        }

        public class Item
        {
            public string kind { get; set; }
            public string etag { get; set; }
            public string id { get; set; }
            public ContentDetails contentDetails { get; set; }
        }


        public string kind { get; set; }
        public string etag { get; set; }
        public PageInfo pageInfo { get; set; }
        public List<Item> items { get; set; }
        
    }
}
