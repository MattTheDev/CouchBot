namespace CB.Data.Entities;

public class CreatorChannel
{
    public long CreatorId { get; set; }
    public string ChannelId { get; set; }
    public int ChannelTypeId { get; set; }
    public string CustomMessage { get; set; }

    public virtual Creator Creator { get; set; }
    public virtual Channel Channel { get; set; }
}