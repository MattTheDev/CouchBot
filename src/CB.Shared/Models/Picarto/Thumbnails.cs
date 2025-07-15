using Newtonsoft.Json;

namespace CB.Shared.Models.Picarto;

public class Thumbnails
{
public string Web { get; set; }

    [JsonProperty("web_large")]
    public string WebLarge { get; set; }

    public string Mobile { get; set; }

    public string Tablet { get; set; }
}