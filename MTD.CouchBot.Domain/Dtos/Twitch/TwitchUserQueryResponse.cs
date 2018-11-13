using Newtonsoft.Json;
using System.Collections.Generic;

namespace MTD.CouchBot.Domain.Dtos.Twitch
{
    public class TwitchUserQueryResponse : ITwitchQueryResponse
    {
        [JsonProperty("data")]
        public List<User> Users { get; set; }

        public class User
        {
            public string Id { get; set; }
            public string Login { get; set; }
            [JsonProperty("display_name")]
            public string DisplayName { get; set; }
            public string Type { get; set; }
            [JsonProperty("broadcaster_type")]
            public string BroadcasterType { get; set; }
            public string Description { get; set; }
            [JsonProperty("profile_image_url")]
            public string ProfileImageUrl { get; set; }
            [JsonProperty("offline_image_url")]
            public string OfflineImageUrl { get; set; }
            [JsonProperty("view_count")]
            public int ViewCount { get; set; }
        }
    }
}
