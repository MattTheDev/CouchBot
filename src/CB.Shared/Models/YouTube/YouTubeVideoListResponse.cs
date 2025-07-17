namespace CB.Shared.Models.YouTube;

public class YouTubeVideoListResponse
{
    public List<Item> Items { get; set; }
    
    public class Snippet
    {
        public string LiveBroadcastContent { get; set; }
    }
    
    public class Item
    {
        public Snippet Snippet { get; set; }
    }
}