using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MTD.CouchBot.Domain.Dtos.YouTube
{
    public class YouTubeChannelQueryByChannelIdResponse
    {
        public string Kind { get; set; }
        public string Etag { get; set; }
        [JsonProperty("pageInfo")]
        public PageInfo _PageInfo { get; set; }
        public List<Item> Items { get; set; }

        public class PageInfo
        {
            public int TotalResults { get; set; }
            public int ResultsPerPage { get; set; }
        }

        public class Default
        {
            public string Url { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
        }

        public class Medium
        {
            public string Url { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
        }

        public class High
        {
            public string Url { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
        }

        public class Thumbnails
        {
            public Default Default { get; set; }
            public Medium Medium { get; set; }
            public High High { get; set; }
        }

        public class Localized
        {
            public string Title { get; set; }
            public string Description { get; set; }
        }

        public class Snippet
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public DateTime PublishedAt { get; set; }
            public Thumbnails Thumbnails { get; set; }
            public Localized Localized { get; set; }
        }

        public class Item
        {
            public string Kind { get; set; }
            public string Etag { get; set; }
            public string Id { get; set; }
            public Snippet Snippet { get; set; }
            public ContentDetails ContentDetails { get; set; }
            public Statistics Statistics { get; set; }
        }

        public class RelatedPlaylists
        {
            public string Uploads { get; set; }
            public string WatchHistory { get; set; }
            public string WatchLater { get; set; }
        }

        public class ContentDetails
        {
            public RelatedPlaylists RelatedPlaylists { get; set; }
        }

        public class Statistics
        {
            public string ViewCount { get; set; }
            public string CommentCount { get; set; }
            public string SubscriberCount { get; set; }
            public bool HiddenSubscriberCount { get; set; }
            public string VideoCount { get; set; }
        }
    }
}