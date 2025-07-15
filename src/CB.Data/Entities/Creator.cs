namespace CB.Data.Entities;

public class Creator
{
    public long Id { get; set; }

    public string ChannelId { get; set; }

    public string DisplayName { get; set; }

    public int PlatformId { get; set; }

    public string UserId { get; set; }

    public bool IsLive { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime ModifiedDate { get; set; }

    public virtual User User { get; set; }
}