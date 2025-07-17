using System.ComponentModel.DataAnnotations;

namespace CB.Data.Entities;

public class Channel
{
    [Key]
    public string Id { get; set; }

public string DisplayName { get; set; }
    public string GuildId { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }

    public virtual Guild Guild { get; set; }

    public virtual ICollection<CreatorChannel> CreatorChannels { get; set; } = new List<CreatorChannel>();
}