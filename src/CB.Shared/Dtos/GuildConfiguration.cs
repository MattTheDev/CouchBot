namespace CB.Shared.Dtos;

public class GuildConfigurationDto
{
public string GuildId { get; set; }

    public bool TextAnnouncements { get; set; }
    public bool DeleteOffline { get; set; }

    public virtual GuildDto Guild { get; set; }
}