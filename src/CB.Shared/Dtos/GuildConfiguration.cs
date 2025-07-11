using System.ComponentModel.DataAnnotations;

namespace CB.Shared.Dtos;

public class GuildConfigurationDto
{
    [Key]
    public string GuildId { get; set; }

    public bool TextAnnouncements { get; set; }
    public bool DeleteOffline { get; set; }

    public virtual GuildDto Guild { get; set; }
}