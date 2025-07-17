namespace CB.Shared.Dtos;

public class TeamChannelDto
{
    public int TeamId { get; set; }
    public string ChannelId { get; set; }

    public TeamDto Team { get; set; }
    public ChannelDto Channel { get; set; }
}