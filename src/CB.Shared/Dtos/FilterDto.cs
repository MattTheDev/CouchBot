namespace CB.Shared.Dtos;

public class FilterDto
{
    public int Id { get; set; }
    public string Text { get; set; }
    public int? PlatformId { get; set; }
    public int FilterTypeId { get; set; }
    public string GuildId { get; set; }

    public PlatformDto? Platform { get; set; }
    public FilterTypeDto FilterType { get; set; }
    public GuildDto Guild { get; set; }
}