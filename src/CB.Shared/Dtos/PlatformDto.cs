namespace CB.Shared.Dtos;

public class PlatformDto
{
    public int Id { get; set; }
    public string DisplayName { get; set; }
    public string SiteUrl { get; set; }
    public string LogoUrl { get; set; }
    public bool Enabled { get; set; }
}