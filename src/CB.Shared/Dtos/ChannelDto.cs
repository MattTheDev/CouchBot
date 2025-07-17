using System.ComponentModel.DataAnnotations;
using CB.Data.Entities;

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

    public virtual ICollection<CreatorChannelDto> CreatorChannels { get; set; } = new List<CreatorChannelDto>();
}