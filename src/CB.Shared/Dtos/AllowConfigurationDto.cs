namespace CB.Shared.Dtos;

public class AllowConfigurationDto
{
    public string GuildId { get; set; }

    public bool AllowLive { get; set; }
    public bool AllowPublished { get; set; }
    public bool AllowThumbnails { get; set; }
    public bool AllowGreetings { get; set; }
    public bool AllowGoodbyes { get; set; }
    public bool AllowLiveDiscovery { get; set; }
    public bool AllowStreamVod { get; set; }
    public bool AllowFfa { get; set; }
    public bool AllowCrosspost { get; set; }
    public bool AllowDiscordLive { get; set; }

    //public virtual GuildDto Guild { get; set; }
}