namespace CB.Shared.Models.YouTube;

public class YouTubeSearchListChannel
{
    public List<Item> Items { get; set; }

    public class High
    {
        public string Url { get; set; }
    }

  public class Thumbnails
    {
        public High High { get; set; }
    }

    public class Snippet
    {
        public string Title { get; set; }
        public Thumbnails Thumbnails { get; set; }
    }

    public class Item
    {
        public string Id { get; set; }
        public Snippet Snippet { get; set; }
        public LiveStreamingDetails LiveStreamingDetails { get; set; }
    }

    public class LiveStreamingDetails
    {
        public DateTime? ActualStartTime { get; set; }
        public DateTime ScheduledStartTime { get; set; }
        public DateTime? ActualEndTime { get; set; }
        public string ConcurrentViewers { get; set; }
        public string ActiveLiveChatId { get; set; }
    }
}