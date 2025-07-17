namespace CB.Data.Entities;

public class CreatorChannel
{
    public long CreatorId { get; set; }
    public string ChannelId { get; set; }
    public int ChannelTypeId { get; set; }
    public string CustomMessage { get; set; }

    public Creator Creator { get; set; }
    public Channel Channel { get; set; }
}