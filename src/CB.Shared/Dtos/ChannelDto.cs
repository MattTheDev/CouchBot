using System.ComponentModel.DataAnnotations;

namespace CB.Shared.Dtos;

public class ChannelDto
{
    [Key]
    public string Id { get; set; }

public string DisplayName { get; set; }
    public string GuildId { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }

    public virtual GuildDto Guild { get; set; }
}