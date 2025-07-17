namespace CB.Shared.Dtos;

public class LiveEmbedDto
{
    public string GuildId { get; set; }

    public string Header { get; set; }

    public string Description { get; set; }

    public string LastStreamed { get; set; }

    public string AverageStream { get; set; }

    public string DescriptionLabel { get; set; }

    public string StreamDescription { get; set; }

    public string FooterStart { get; set; }

    public string FooterStopped { get; set; }

    public string ChannelButton { get; set; }

    public virtual GuildDto Guild { get; set; }
}