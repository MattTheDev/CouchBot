using System.ComponentModel.DataAnnotations;

namespace CB.Data.Entities;

public class ChannelConfiguration
{
    [Key]
    public string GuildId { get; set; }

    public string? GreetingChannelId { get; set; }
    public string? GoodbyeChannelId { get; set; }
    public string? LiveChannelId { get; set; }
    public string? DiscordLiveChannelId { get; set; }

    public virtual Guild Guild { get; set; }
    public virtual Channel GreetingChannel { get; set; }
    public virtual Channel GoodbyeChannel { get; set; }
    public virtual Channel LiveChannel { get; set; }
    public virtual Channel DiscordLiveChannel { get; set; }
}