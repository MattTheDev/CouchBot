namespace CB.Data.Entities;

public class TeamChannel
{
    public int TeamId { get; set; }
    public string ChannelId { get; set; }

    public Team Team { get; set; }
    public Channel Channel { get; set; }
}