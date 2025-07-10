using System.ComponentModel.DataAnnotations;

namespace CB.Data.Entities;

public class RoleConfiguration
{
    [Key]
    public string GuildId { get; set; }

    public string JoinRoleId { get; set; }
    public string DiscoveryRoleId { get; set; }
    public string LiveDiscoveryRoleId { get; set; }

    public virtual Guild Guild { get; set; }
}