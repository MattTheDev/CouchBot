using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;

namespace MTD.CouchBot.Domain.Models.Bot
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
        public List<string> ServerVidMeChannels { get; set; }
        public List<int> ServerVidMeChannelIds { get; set; }
        public List<string> PicartoChannels { get; set; }
        public float TimeZoneOffset { get; set; }
        public bool AllowThumbnails { get; set; }
        public bool Greetings { get; set; }
        public bool Goodbyes { get; set; }
        public string GreetingMessage { get; set; }
        public string GoodbyeMessage { get; set; }
        public bool AllowPublished { get; set; }
        public string PublishedMessage { get; set; }
        public string LiveMessage { get; set; }
        public bool UseYouTubeGamingPublished { get; set; }
        public bool UseTextAnnouncements { get; set; }
        public bool DeleteWhenOffline { get; set; }
        public ulong MentionRole { get; set; }
        public bool AllowLive { get; set; }
        public string OwnerBeamChannel { get; set; }
        public string OwnerBeamChannelId { get; set; }
        public string OwnerHitboxChannel { get; set; }
        public string OwnerPicartoChannel { get; set; }
        public string OwnerTwitchChannel { get; set; }
        public string OwnerTwitchChannelId { get; set; }
        public string OwnerYouTubeChannelId { get; set; }
        public string OwnerVidMeChannel { get; set; }
        public int OwnerVidMeChannelId { get; set; }
        public ulong OwnerTwitchFeedChannel { get; set; }
        public ulong TwitchFeedChannel { get; set; }
        public bool AllowChannelFeed { get; set; }
        public bool AllowOwnerChannelFeed { get; set; }
        public string StreamOfflineMessage { get; set; }
        public List<ulong> ApprovedAdmins { get; set; }
        public List<string> TwitchTeams { get; set; }
        public List<string> TwitchGames { get; set; }
        public List<string> ServerGameList { get; set; }
    }
}
