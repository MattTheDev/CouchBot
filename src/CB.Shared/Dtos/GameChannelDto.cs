namespace CB.Shared.Dtos;

public class GameChannelDto
{
    public int GameId { get; set; }
    public string ChannelId { get; set; }

    public GameDto Game { get; set; }
    public ChannelDto Channel { get; set; }
}