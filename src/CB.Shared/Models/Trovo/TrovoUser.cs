using Newtonsoft.Json;

namespace CB.Shared.Models.Trovo;

public class TrovoUser
{
    public int Total { get; set; }
    public List<User> Users { get; set; }

    public class User
    {
        [JsonProperty("user_id")]
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Nickname { get; set; }
        [JsonProperty("channel_id")]
        public string ChannelId { get; set; }
    }
}