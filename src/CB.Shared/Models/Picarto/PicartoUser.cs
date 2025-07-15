using Newtonsoft.Json;

namespace CB.Shared.Models.Picarto;

public class PicartoUser
{
    [JsonProperty("channel_details")]
    public ChannelDetails ChannelDetails { get; set; }

    public string Email { get; set; }

    [JsonProperty("creation_date")]
    public string CreationDate { get; set; }

    [JsonProperty("private_key")]
    public string PrivateKey { get; set; }

    [JsonProperty("nsfw_enabled")]
    public bool NsfwEnabled { get; set; }

    [JsonProperty("nsfw_app")]
    public bool NsfwApp { get; set; }
}