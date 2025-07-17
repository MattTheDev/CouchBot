using System.ComponentModel.DataAnnotations;

namespace CB.Shared.Dtos;

public class GuildDto
{
    [Key]
    public string Id { get; set; }

    public string DisplayName { get; set; }

    public string OwnerId { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime ModifiedDate { get; set; }

    public virtual UserDto Owner { get; set; }
    public virtual ICollection<ChannelDto> Channels { get; set; }
    public virtual AllowConfigurationDto AllowConfiguration { get; set; }
    public virtual ChannelConfigurationDto ChannelConfiguration { get; set; }
    public virtual GuildConfigurationDto GuildConfiguration { get; set; }
    public virtual MessageConfigurationDto MessageConfiguration { get; set; }
    public virtual RoleConfigurationDto RoleConfiguration { get; set; }
    public virtual ClipEmbedDto ClipEmbed { get; set; }
    public virtual LiveEmbedDto LiveEmbed { get; set; }
    public virtual VodEmbedDto VodEmbed { get; set; }
}