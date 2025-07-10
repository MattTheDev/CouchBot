using System.ComponentModel.DataAnnotations;

namespace CB.Data.Entities;

public class GuildConfiguration
{
    [Key]
    public string GuildId { get; set; }

    public bool TextAnnouncements { get; set; }
    public bool DeleteOffline { get; set; }

    public virtual Guild Guild { get; set; }
}