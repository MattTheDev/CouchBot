namespace CB.Shared.Dtos;

public class RoleConfigurationDto
{
    public string GuildId { get; set; }

    public string JoinRoleId { get; set; }
    public string DiscoveryRoleId { get; set; }
    public string LiveDiscoveryRoleId { get; set; }

    //public virtual GuildDto Guild { get; set; }
}