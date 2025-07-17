namespace CB.Shared.Models.YouTube;

public class LiveChatStatusResponse
{
    public List<Item> Items { get; set; }

    public class Snippet
    {
        public string ChannelId { get; set; }

        public string CurrentVideoId { get; set; }
    }

    public class Item
    {
        public Snippet Snippet { get; set; }
    }
}