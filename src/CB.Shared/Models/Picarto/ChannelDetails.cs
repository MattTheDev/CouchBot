using Newtonsoft.Json;

namespace CB.Shared.Models.Picarto;

public class ChannelDetails
{
    [JsonProperty("user_id")]
    public int UserId { get; set; }

    public string Name { get; set; }

    public string Avatar { get; set; }

    public bool Online { get; set; }

    public int Viewers { get; set; }

    [JsonProperty("viewers_total")]
    public int ViewersTotal { get; set; }

    public Thumbnails Thumbnails { get; set; }

    public int Followers { get; set; }

    public int Subscribers { get; set; }

    public bool Adult { get; set; }

    public List<string> Category { get; set; }

    [JsonProperty("account_type")]
    public string AccountType { get; set; }

    public bool Commissions { get; set; }

    public bool Recordings { get; set; }

    public string Title { get; set; }

    [JsonProperty("description_panels")]
    public List<object> DescriptionPanels { get; set; }

    public bool Private { get; set; }

    [JsonProperty("private_message")]
    public string PrivateMessage { get; set; }

    public bool Gaming { get; set; }

    [JsonProperty("chat_settings")]
    public ChatSettings ChatSettings { get; set; }

    [JsonProperty("last_live")]
    public object LastLive { get; set; }

    public List<object> Tags { get; set; }

    public object Multistream { get; set; }

    public List<Language> Languages { get; set; }

    public bool Following { get; set; }

    [JsonProperty("creation_date")]
    public string CreationDate { get; set; }
}