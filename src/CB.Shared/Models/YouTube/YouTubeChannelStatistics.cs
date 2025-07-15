namespace CB.Shared.Models.YouTube;

public class YouTubeChannelStatistics
{
    public class Statistics
    {
        public string ViewCount { get; set; }
        public string CommentCount { get; set; }
        public string SubscriberCount { get; set; }
        public bool HiddenSubscriberCount { get; set; }
        public string VideoCount { get; set; }
    }

    public class Item
    {
        public string Kind { get; set; }
        public string Etag { get; set; }
        public string Id { get; set; }
        public Statistics Statistics { get; set; }
    }

    public string Kind { get; set; }
    public string Etag { get; set; }
    public PageInfo PageInfo { get; set; }
    public List<Item> Items { get; set; }
}