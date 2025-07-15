namespace CB.Shared.Models.YouTube;

public class YouTubeChannelByIdResponse
{
    public class Default
    {
        public string Url { get; set; }
    }

    public class Medium
    {
        public string Url { get; set; }
    }

    public class High
    {
        public string Url { get; set; }
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
        public string PublishedAt { get; set; }
        public Thumbnails Thumbnails { get; set; }
        public Localized Localized { get; set; }
        public string Country { get; set; }
    }

    public class Item
    {
        public string Kind { get; set; }
        public string Etag { get; set; }
        public string Id { get; set; }
        public Snippet Snippet { get; set; }
        public ContentDetails ContentDetails { get; set; }
    }

    public class ContentDetails
    {
        public RelatedPlaylists RelatedPlaylists { get; set; }
    }

    public class RelatedPlaylists
    {
        public string Likes { get; set; }
        public string Uploads { get; set; }
        public string WatchHistory { get; set; }
        public string WatchLater { get; set; }
    }

    public string Kind { get; set; }
    public string Etag { get; set; }
    public PageInfo PageInfo { get; set; }
    public List<Item> Items { get; set; }
}