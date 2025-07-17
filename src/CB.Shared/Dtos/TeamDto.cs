namespace CB.Shared.Dtos;

public class TeamDto
{
    public int Id { get; set; }

    public string DisplayName { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
    public int PlatformId { get; set; }
    public string TeamId { get; set; }

    public virtual PlatformDto Platform { get; set; }
    public ICollection<TeamChannelDto> TeamChannels { get; set; }
}