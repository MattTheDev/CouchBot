namespace CB.Shared.Dtos;

public class VodEmbedDto
{
    public string GuildId { get; set; }

    public string Header { get; set; }

    public string DescriptionLabel { get; set; }

    public string Description { get; set; }

    public string Footer { get; set; }

    public string ChannelButton { get; set; }

    public virtual GuildDto Guild { get; set; }
}