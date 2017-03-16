using System.Collections.Generic;

namespace MTD.DiscordBot.Json
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
        public bool AllowEveryone { get; set; }
        public List<string> Users { get; set; }
        public bool BroadcastOthers { get; set; }
        public List<string> BroadcasterWhitelist { get; set; }
        public bool UseWhitelist { get; set; }
        public bool BroadcastSubGoals { get; set; }
        public List<string> ServerYouTubeChannelIds { get; set; }
        public List<string> ServerTwitchChannels { get; set; }
        public List<string> ServerTwitchChannelIds { get; set; }
        public List<string> ServerBeamChannels { get; set; }
        public List<string> ServerHitboxChannels { get; set; }
        public float TimeZoneOffset { get; set; }
        public bool AllowThumbnails { get; set; }
        public bool Greetings { get; set; }
        public bool Goodbyes { get; set; }
        public string GreetingMessage { get; set; }
        public string GoodbyeMessage { get; set; }
        public bool AllowPublished { get; set; }
        public bool AllowPublishedOthers { get; set; }
        public ulong PublishedChannel { get; set; }
        public string PublishedMessage { get; set; }
        public string LiveMessage { get; set; }
        public bool UseYouTubeGamingPublished { get; set; }
        public bool UseTextAnnouncements { get; set; }
        public bool DeleteWhenOffline { get; set; }
    }
}
