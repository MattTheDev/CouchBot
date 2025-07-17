namespace CB.Shared.Dtos;

public class CreatorChannelDto
{
    public long CreatorId { get; set; }
    public string ChannelId { get; set; }
    public int ChannelTypeId { get; set; }
    public string CustomMessage { get; set; }

    public CreatorDto Creator { get; set; }
    public ChannelDto Channel { get; set; }
}