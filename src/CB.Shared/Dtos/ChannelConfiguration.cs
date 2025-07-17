namespace CB.Shared.Dtos;

public class ChannelConfigurationDto
{
    public string GuildId { get; set; }

    public string? GreetingChannelId { get; set; }
    public string? GoodbyeChannelId { get; set; }
    public string? LiveChannelId { get; set; }
    public string? DiscordLiveChannelId { get; set; }

    public virtual GuildDto Guild { get; set; }
    public virtual ChannelDto GreetingChannel { get; set; }
    public virtual ChannelDto GoodbyeChannel { get; set; }
    public virtual ChannelDto LiveChannel { get; set; }
    public virtual ChannelDto DiscordLiveChannel { get; set; }
}