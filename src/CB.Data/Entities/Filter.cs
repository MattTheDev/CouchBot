namespace CB.Data.Entities;

public class Filter
{
    public int Id { get; set; }
    public string Text { get; set; }
    public int PlatformId { get; set; }
    public int FilterTypeId { get; set; }
    public string GuildId { get; set; }

    public Platform Platform { get; set; }
    public FilterType FilterType { get; set; }
    public Guild Guild { get; set; }
}