using Newtonsoft.Json;

namespace CB.Shared.Models.Picarto;

public class PicartoChannel
{
    [JsonProperty("channel")]
    public Channel ChannelData;

    [JsonProperty("getLoadBalancerUrl")]
    public GetLoadBalancerUrl GetLoadBalancerUrlData;

    [JsonProperty("getMultiStreams")]
    public GetMultiStreams GetMultiStreamsData;

    public class Category
    {
public string Name;
    }

    public class Channel
    {
        public bool Online;

        public string Color;

        [JsonProperty("dm_setting")]
        public DmSetting DmSetting;

        [JsonProperty("created_at")]
        public string CreatedAt;

        public bool Notification;

        public bool Gifted;

        [JsonProperty("gifted_channel_name")]
        public object GiftedChannelName;

        public int Id;

        [JsonProperty("account_type")]
        public string AccountType;

        public bool Adult;

        public string Avatar;

        public string Bio;

        public int Columns;

        public bool Commissions;

        [JsonProperty("enable_subscription")]
        public bool EnableSubscription;

        public bool Gaming;

        public string Name;

        public bool Private;

        [JsonProperty("show_subscriber_count")]
        public bool ShowSubscriberCount;

        public string Title;

        public bool Verified;

        public bool Streaming;

        [JsonProperty("total_views")]
        public int TotalViews;

        public int Viewers;

        public List<object> Descriptions;

        public List<object> Banners;

        public List<Category> Categories;

        public List<Language> Languages;

        [JsonProperty("social_medias")]
        public List<object> SocialMedias;

        public List<Software> Softwares;

        public List<object> Tags;

        public List<object> Tools;

        [JsonProperty("followers_count")]
        public int FollowersCount;

        [JsonProperty("videos_count")]
        public int VideosCount;
    }

    public class DmSetting
    {
        [JsonProperty("allow_message_every_one")]
        public bool AllowMessageEveryOne;
    }

    public class GetLoadBalancerUrl
    {
        public string Url;

        public string Origin;
    }

    public class GetMultiStreams
    {
        public string Name;

        public bool Streaming;

        public bool Online;

        public int Viewers;

        public bool Multistream;

        public List<Stream> Streams;

        public string DisplayName;
    }

    public class Language
    {
        public int Id;

public string Name;

        public string Code;

        public object Image;
    }

    public class Software
    {
        public int Id;

        public string Name;
    }

    public class Stream
    {
        public int Id;

        [JsonProperty("user_id")]
        public int UserId;

        public string Name;

        [JsonProperty("account_type")]
        public string AccountType;

        public string Avatar;

        [JsonProperty("offline_image")]
        public object OfflineImage;

        public bool Streaming;

        public bool Adult;

        public bool Multistream;

        public int Viewers;

        public bool Hosted;

        public bool Host;

        public bool Following;

        public bool Subscription;

        public bool Online;

        public int ChannelId;

        [JsonProperty("stream_name")]
        public string StreamName;

        public string Color;

        public bool Webrtc;

        [JsonProperty("subscription_enabled")]
        public bool SubscriptionEnabled;

        [JsonProperty("thumbnail_image")]
        public string ThumbnailImage;
    }
}