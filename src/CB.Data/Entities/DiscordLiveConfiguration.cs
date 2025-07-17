using System.ComponentModel.DataAnnotations;

namespace CB.Data.Entities;

public class DiscordLiveConfiguration
{
    [Key]
    public string GuildId { get; set; }

    public string Message { get; set; }
    public string Header { get; set; }
    public string Description { get; set; }
    public string Footer { get; set; }
    public string MentionRoleId { get; set; }

    public virtual Guild Guild { get; set; }
}