using System.ComponentModel.DataAnnotations;
using CB.Data.Entities;

namespace CB.Shared.Dtos;

public class DiscordLiveConfigurationDto
{
    public string GuildId { get; set; }

    public string Message { get; set; }
    public string Header { get; set; }
    public string Description { get; set; }
    public string Footer { get; set; }
    public string MentionRoleId { get; set; }

    public virtual GuildDto Guild { get; set; }
}