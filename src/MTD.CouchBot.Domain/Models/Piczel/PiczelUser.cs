using Newtonsoft.Json;

namespace MTD.CouchBot.Domain.Models.Piczel
{
    public class PiczelUser
    {
        public int Id { get; set; }
        public string Username { get; set; }
        [JsonProperty("premium?")]
        public bool IsPremium { get; set; }
        [JsonProperty("avatar")]
        public PiczelAvatar Avatar { get; set; }
        public string Role { get; set; }
        public PiczelGallery Gallery { get; set; }
        [JsonProperty("follower_count")]
        public int FollowerCount { get; set; }
}
}
