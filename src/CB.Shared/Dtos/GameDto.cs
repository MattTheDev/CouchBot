namespace CB.Shared.Dtos;

public class GameDto
{
    public int Id { get; set; }

    public string DisplayName { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
    public int PlatformId { get; set; }
public string GameId { get; set; }

public virtual PlatformDto Platform { get; set; }
    public virtual ICollection<GameChannelDto> GameChannels { get; set; }
}