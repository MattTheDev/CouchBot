using System.ComponentModel.DataAnnotations;

namespace CB.Data.Entities;

public class ClipEmbed
{
    [Key]
    public string GuildId { get; set; }

    public string Header { get; set; }

    public string Description { get; set; }

    public string Footer { get; set; }

    public string WatchButton { get; set; }

    public string MoreButton { get; set; }

    public virtual Guild Guild { get; set; }
}