using System.ComponentModel.DataAnnotations;

namespace CB.Data.Entities;

public class Platform
{
    [Key]
    public int Id { get; set; }
    public string DisplayName { get; set; }
    public string SiteUrl { get; set; }
    public string LogoUrl { get; set; }
    public bool Enabled { get; set; }
}