using Newtonsoft.Json;

namespace CB.Shared.Models.YouTube;

public class YouTubePlaylist
{
    public string Kind { get; set; }

    public string Etag { get; set; }

    public string NextPageToken { get; set; }

    public List<Item> Items { get; set; }

    [JsonProperty("pageInfo")]
    public PageInfo NextPageInfo { get; set; }

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

    public class Standard
    {
        public string Url { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }
    }

    public class Maxres
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

        public Standard Standard { get; set; }

        public Maxres Maxres { get; set; }
    }

    public class ResourceId
    {
        public string Kind { get; set; }

        public string VideoId { get; set; }
    }

    public class Snippet
    {
        public DateTime PublishedAt { get; set; }

        public string ChannelId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public Thumbnails Thumbnails { get; set; }

        public string ChannelTitle { get; set; }

        public string PlaylistId { get; set; }

        public int Position { get; set; }

        public ResourceId ResourceId { get; set; }

        public string VideoOwnerChannelTitle { get; set; }

        public string VideoOwnerChannelId { get; set; }
    }

    public class ContentDetails
    {
        public string VideoId { get; set; }

        public DateTime VideoPublishedAt { get; set; }
    }

    public class Status
    {
        public string PrivacyStatus { get; set; }
    }

    public class Item
    {
        public string Kind { get; set; }

        public string Etag { get; set; }

        public string Id { get; set; }

        public Snippet Snippet { get; set; }

        public ContentDetails ContentDetails { get; set; }

        public Status Status { get; set; }
    }
}