namespace CB.Shared.Dtos;

public class UserDto
{
public string Id { get; set; }

    public string DisplayName { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime ModifiedDate { get; set; }

    public virtual ICollection<GuildDto> Guilds { get; set; }
    public virtual ICollection<CreatorDto> Creators { get; set; }
}