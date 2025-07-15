using Newtonsoft.Json;

namespace CB.Shared.Models.Picarto;

public class Channel
{
public string Avatar { get; set; }

    public string Color { get; set; }

    [JsonProperty("created_at")]
    public string CreatedAt { get; set; }

    public string Name { get; set; }

    public bool Online { get; set; }

    public string Title { get; set; }

    [JsonProperty("total_views")]
    public int TotalViews { get; set; }

    public int Viewers { get; set; }

    [JsonProperty("followers_count")]
    public int FollowersCount { get; set; }

    [JsonProperty("subscribers_count")]
    public int SubscribersCount { get; set; }

    [JsonProperty("avatar_url")]
    public string AvatarUrl { get; set; }

    [JsonProperty("image_thumbnail")]
    public string ImageThumbnail { get; set; }

    public string DisplayName { get; set; }
}