using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CB.Shared.Dtos;

public class UserDto
{
    [Key]
    public string Id { get; set; }

    public string DisplayName { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime CreatedDate { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime ModifiedDate { get; set; }

    public virtual ICollection<GuildDto> Guilds { get; set; }
    public virtual ICollection<CreatorDto> Creators { get; set; }
}