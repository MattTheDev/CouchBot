using Newtonsoft.Json;
using System.Collections.Generic;

namespace MTD.CouchBot.Domain.Models.Picarto
{
    public class PicartoChannel
    {
        [JsonProperty("user_id")]
        public int UserId { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("online")]
        public bool Online { get; set; }
        [JsonProperty("viewers")]
        public int Viewers { get; set; }
        [JsonProperty("viewers_total")]
        public int ViewersTotal { get; set; }
        [JsonProperty("followers")]
        public int Followers { get; set; }
        [JsonProperty("adult")]
        public bool Adult { get; set; }
        [JsonProperty("category")]
        public string Category { get; set; }
        [JsonProperty("account_type")]
        public string AccountType { get; set; }
        [JsonProperty("commissions")]
        public bool Commissions { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("description_panels")]
        public List<object> DescriptionPanels { get; set; }
        [JsonProperty("private")]
        public bool IsPrivate { get; set; }
        [JsonProperty("gaming")]
        public bool Gaming { get; set; }
        [JsonProperty("guest_chat")]
        public bool GuestChat { get; set; }
        [JsonProperty("last_live")]
        public string LastLive { get; set; }
        [JsonProperty("tags")]
        public List<string> Tags { get; set; }
        [JsonProperty("multistream")]
        public List<PicartoMultistream> Multistream { get; set; }
    }
}
