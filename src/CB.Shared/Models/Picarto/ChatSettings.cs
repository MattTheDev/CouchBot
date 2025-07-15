using Newtonsoft.Json;

namespace CB.Shared.Models.Picarto;

public class ChatSettings
{
    [JsonProperty("guest_chat")]
    public bool GuestChat { get; set; }

    public bool Links { get; set; }

    public bool Level { get; set; }
}