using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;

namespace MTD.CouchBot.Json
{
    public class DiscordServer
    {
        public ulong Id { get; set; }
        public string Name { get; set; }
        public ulong OwnerId { get; set; }
        public string OwnerName { get; set; }
        public ulong AnnouncementsChannel { get; set; }
        public ulong GoLiveChannel { get; set; }
        public ulong GreetingsChannel { get; set; }
        public ulong OwnerLiveChannel { get; set; }
        public ulong OwnerPublishedChannel { get; set; }
        public ulong PublishedChannel { get; set; }
        public bool AllowEveryone { get; set; }
        public List<string> Users { get; set; }
        public bool BroadcastSubGoals { get; set; }
        public List<string> ServerYouTubeChannelIds { get; set; }
        public List<string> ServerTwitchChannels { get; set; }
        public List<string> ServerTwitchChannelIds { get; set; }
        public List<string> ServerBeamChannels { get; set; }
        public List<string> ServerBeamChannelIds { get; set; }
        public List<string> ServerHitboxChannels { get; set; }
        public float TimeZoneOffset { get; set; }
        public bool AllowThumbnails { get; set; }
        public bool Greetings { get; set; }
        public bool Goodbyes { get; set; }
        public string GreetingMessage { get; set; }
        public string GoodbyeMessage { get; set; }

        [DefaultValue(true)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool AllowPublished { get; set; }
        public string PublishedMessage { get; set; }
        public string LiveMessage { get; set; }
        public bool UseYouTubeGamingPublished { get; set; }
        public bool UseTextAnnouncements { get; set; }
        public bool DeleteWhenOffline { get; set; }
        public ulong MentionRole { get; set; }
        [DefaultValue(true)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool AllowLive { get; set; }
        public string OwnerBeamChannel { get; set; }
        public string OwnerBeamChannelId { get; set; }
        public string OwnerHitboxChannel { get; set; }
        public string OwnerTwitchChannel { get; set; }
        public string OwnerTwitchChannelId { get; set; }
        public string OwnerYouTubeChannelId { get; set; }
        public ulong OwnerTwitchFeedChannel { get; set; }
        public ulong TwitchFeedChannel { get; set; }

        [DefaultValue(false)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool AllowChannelFeed { get; set; }

        [DefaultValue(false)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public bool AllowOwnerChannelFeed { get; set; }

        [DefaultValue("This stream is now offline.")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public string StreamOfflineMessage { get; set; }
    }
}
