namespace CB.Data.Entities;

public class GameChannel
{
    public int GameId { get; set; }
    public string ChannelId { get; set; }

    public Game Game { get; set; }
    public Channel Channel { get; set; }
}