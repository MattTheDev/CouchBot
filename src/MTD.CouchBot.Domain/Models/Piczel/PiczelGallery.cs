using Newtonsoft.Json;

namespace MTD.CouchBot.Domain.Models.Piczel
{
    public class PiczelGallery
    {
        [JsonProperty("profile_description")]
        public string ProfileDescription { get; set; }
    }
}
