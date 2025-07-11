namespace CB.Shared.Dtos;

public class CreatorDto
{
    public long Id { get; set; }

    public string ChannelId { get; set; }

    public string DisplayName { get; set; }

    public int PlatformId { get; set; }

    public string UserId { get; set; }

    public bool IsLive { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime ModifiedDate { get; set; }

    public virtual UserDto User { get; set; }
}