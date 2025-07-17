namespace CB.Shared.Models.Bot;

public class YouTubeCreatorDropdownPayload : GenericDropdownPayload
{
    public ulong GuildId { get; set; }
    public ulong ChannelId { get; set; }
    public int ChannelType { get; set; }
    public string CustomMessage { get; set; }
}