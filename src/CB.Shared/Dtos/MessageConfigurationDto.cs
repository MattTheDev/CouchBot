namespace CB.Shared.Dtos;

public class MessageConfigurationDto
{
    public string GuildId { get; set; }

    public string GreetingMessage { get; set; }
    public string GoodbyeMessage { get; set; }
    public string LiveMessage { get; set; }
    public string PublishedMessage { get; set; }
    public string StreamOfflineMessage { get; set; }

    public virtual GuildDto Guild { get; set; }
}