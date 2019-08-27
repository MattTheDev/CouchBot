using System.Collections.Generic;

namespace MTD.CouchBot.Domain.Models.Bot
{
    public class DiscordServer
    {
        public ulong Id { get; set; }
        public string Name { get; set; }
        public ulong OwnerId { get; set; }
        public string OwnerName { get; set; }
        public ulong GoLiveChannel { get; set; }
        public ulong GreetingsChannel { get; set; }
        public ulong OwnerLiveChannel { get; set; }
        public ulong OwnerPublishedChannel { get; set; }
        public ulong PublishedChannel { get; set; }
        public bool AllowEveryone { get; set; }
        public bool BroadcastSubGoals { get; set; }
        public List<string> ServerYouTubeChannelIds { get; set; }
        public List<string> ServerTwitchChannelIds { get; set; }
        public List<string> ServerBeamChannelIds { get; set; }
        public List<string> ServerHitboxChannels { get; set; }
        public List<string> PicartoChannels { get; set; }
        public List<int> ServerPiczelChannelIds { get; set; }
        public float TimeZoneOffset { get; set; }
        public bool AllowThumbnails { get; set; }
        public bool Greetings { get; set; }
        public bool Goodbyes { get; set; }
        public string GreetingMessage { get; set; }
        public string GoodbyeMessage { get; set; }
        public string PublishedMessage { get; set; }
        public string LiveMessage { get; set; }
        public bool UseYouTubeGamingPublished { get; set; }
        public bool UseTextAnnouncements { get; set; }
        public bool DeleteWhenOffline { get; set; }
        public ulong MentionRole { get; set; }
        public bool AllowLive { get; set; }
        public bool AllowPublished { get; set; }
        public bool AllowMentionMixerLive { get; set; } = true;
        public bool AllowMentionMobcrushLive { get; set; } = true;
        public bool AllowMentionPicartoLive { get; set; } = true;
        public bool AllowMentionPiczelLive { get; set; } = true;
        public bool AllowMentionSmashcastLive { get; set; } = true;
        public bool AllowMentionTwitchLive { get; set; } = true;
        public bool AllowMentionYouTubeLive { get; set; } = true;
        public bool AllowMentionOwnerLive { get; set; } = true;
        public bool AllowMentionOwnerMixerLive { get; set; } = true;
        public bool AllowMentionOwnerMobcrushLive { get; set; } = true;
        public bool AllowMentionOwnerPicartoLive { get; set; } = true;
        public bool AllowMentionOwnerPiczelLive { get; set; } = true;
        public bool AllowMentionOwnerSmashcastLive { get; set; } = true;
        public bool AllowMentionOwnerTwitchLive { get; set; } = true;
        public bool AllowMentionOwnerYouTubeLive { get; set; } = true;
        public bool AllowMentionYouTubePublished { get; set; } = true;
        public bool AllowMentionOwnerYouTubePublished { get; set; } = true;
        public bool AllowMature { get; set; } = true;
        public string OwnerBeamChannelId { get; set; }
        public string OwnerHitboxChannel { get; set; }
        public string OwnerPicartoChannel { get; set; }
        public string OwnerTwitchChannelId { get; set; }
        public string OwnerYouTubeChannelId { get; set; }
        public int? OwnerPiczelChannelId { get; set; }
        public string StreamOfflineMessage { get; set; }
        public List<ulong> ApprovedAdmins { get; set; }
        public List<string> TwitchTeams { get; set; }
        public List<int> MixerTeams { get; set; }
        public List<string> ServerGameList { get; set; }
        public List<string> ServerMobcrushIds { get; set; }
        public string OwnerMobcrushId { get; set; }
        public bool AllowMobcrush { get; set; }
        public string DiscoverTwitch { get; set; }
        public ulong DiscoverTwitchRole { get; set; }
        public List<CustomCommand> CustomCommands { get; set; }
        public bool AllowVodcasts { get; set; }
        public long OwnerFacebookId { get; set; }
        public List<long> FacebookIds { get; set; }
        public bool DisplayStreamStatistics { get; set; } = true;
        public string Prefix { get; set; } = "!cb";
        public ulong LiveTwitchRole { get; set; }
        public List<RoleCommand> RoleCommands { get; set; }
        public ulong JoinRole { get; set; }
        public XPromo XPromo { get; set; }
        public Admin Admins { get; set; }

        public List<DiscordStreamer> Streamers { get; set; }
    }
}
