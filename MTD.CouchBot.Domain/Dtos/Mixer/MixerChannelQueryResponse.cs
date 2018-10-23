using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MTD.CouchBot.Domain.Dtos.Mixer
{
    public class MixerChannelQueryResponse
    {
        public bool Featured { get; set; }
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; }
        public bool Online { get; set; }
        public int FeatureLevel { get; set; }
        public bool Partnered { get; set; }
        public int TranscodingProfileId { get; set; }
        public bool Suspended { get; set; }
        public string Name { get; set; }
        public string Audience { get; set; }
        public int ViewersTotal { get; set; }
        public int ViewersCurrent { get; set; }
        public int NumFollowers { get; set; }
        public string Description { get; set; }
        public int TypeId { get; set; }
        public bool Interactive { get; set; }
        public int InteractiveGameId { get; set; }
        public int Ftl { get; set; }
        public bool HasVod { get; set; }
        public string LanguageId { get; set; }
        public int CoverId { get; set; }
        public int ThumbnailId { get; set; }
        public object BadgeId { get; set; }
        public object BannerUrl { get; set; }
        public object HosteeId { get; set; }
        public bool HasTranscodes { get; set; }
        public bool VodsEnabled { get; set; }
        public object CostreamId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public object DeletedAt { get; set; }
        [JsonProperty("thumbnail")]
        public Thumbnail _Thumbnail { get; set; }
        [JsonProperty("cover")]
        public Cover _Cover { get; set; }
        public object Badge { get; set; }
        [JsonProperty("type")]
        public Type _Type { get; set; }
        [JsonProperty("preferences")]
        public Preferences _Preferences { get; set; }
        [JsonProperty("user")]
        public User _User { get; set; }

        public class Meta
        {
            public List<int> Size { get; set; }
        }

        public class Thumbnail
        {
            public Meta Meta { get; set; }
            public int Id { get; set; }
            public string Type { get; set; }
            public int Relid { get; set; }
            public string Url { get; set; }
            public string Store { get; set; }
            public string RemotePath { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
        }

        public class Meta2
        {
            public string Small { get; set; }
        }

        public class Cover
        {
            public Meta2 Meta { get; set; }
            public int Id { get; set; }
            public string Type { get; set; }
            public object Relid { get; set; }
            public string Url { get; set; }
            public string Store { get; set; }
            public string RemotePath { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
        }

        public class Type
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Parent { get; set; }
            public string Description { get; set; }
            public string Source { get; set; }
            public int ViewersCurrent { get; set; }
            public object CoverUrl { get; set; }
            public object BackgroundUrl { get; set; }
            public int Online { get; set; }
            public object AvailableAt { get; set; }
        }

        public class Preferences
        {
            [JsonProperty("hypezone:allow")]
            public bool HypezoneAllow { get; set; }
            [JsonProperty("hosting:allow")]
            public bool HostingAllow { get; set; }
            [JsonProperty("hosting:allowlive")]
            public bool HostingAllowLive { get; set; }
            [JsonProperty("mixer:featured:allow")]
            public bool MixerFeaturedAllow { get; set; }
            [JsonProperty("costream:allow")]
            public string CostreamAllow { get; set; }
            public string Sharetext { get; set; }
            [JsonProperty("channel:bannedwords")]
            public List<object> ChannelBannedWords { get; set; }
            [JsonProperty("channel:links:clickable")]
            public bool ChannelLinksClickable { get; set; }
            [JsonProperty("channel:links:allowed")]
            public bool ChannelLinksAllowed { get; set; }
            [JsonProperty("channel:slowchat")]
            public int ChannelSlowChat { get; set; }
            [JsonProperty("channel:notify:directPurchaseMessage")]
            public string ChannelNotifyDirectPurchaseMessage { get; set; }
            [JsonProperty("channel:notify:directPurchase")]
            public bool ChannelNotifyDirectPurchase { get; set; }
            [JsonProperty("channel:notify:follow")]
            public bool ChannelNotifyFollow { get; set; }
            [JsonProperty("channel:notify:followmessage")]
            public string ChannelNotifyFollowMessage { get; set; }
            [JsonProperty("channel:notify:hostedBy")]
            public string ChannelNotifyHostedBy { get; set; }
            [JsonProperty("channel:notify:hosting")]
            public string ChannelNotifyHosting { get; set; }
            [JsonProperty("channel:notify:subscribemessage")]
            public string ChannelNotifySubscribeMessage { get; set; }
            [JsonProperty("channel:notify:subscribe")]
            public bool ChannelNotifySubscribe { get; set; }
            [JsonProperty("channel:partner:submail")]
            public string ChannelPartnerSubmail { get; set; }
            [JsonProperty("channel:player:muteOwn")]
            public bool ChannelPlayerMuteOwn { get; set; }
            [JsonProperty("channel:tweet:enabled")]
            public bool ChannelTweetEnabled { get; set; }
            [JsonProperty("channel:tweet:body")]
            public string ChannelTweetBody { get; set; }
            [JsonProperty("channel:users:levelRestrict")]
            public int ChannelUsersLevelRestrict { get; set; }
            [JsonProperty("channel:catbot:level")]
            public int ChannelCatbotLevel { get; set; }
            [JsonProperty("channel:offline:autoplayVod")]
            public bool ChannelOfflineAutoPlayVod { get; set; }
            [JsonProperty("channel:chat:hostswitch")]
            public bool ChannelChatHostSwitch { get; set; }
            [JsonProperty("channel:directPurchase:enabled")]
            public bool ChannelDirectPurchaseEnabled { get; set; }
        }

        public class Social
        {
            public string Twitter { get; set; }
            public List<object> Verified { get; set; }
        }

        public class Group
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class User
        {
            public int Level { get; set; }
            public Social Social { get; set; }
            public int Id { get; set; }
            public string Username { get; set; }
            public bool Verified { get; set; }
            public int Experience { get; set; }
            public int Sparks { get; set; }
            public string AvatarUrl { get; set; }
            public string Bio { get; set; }
            public object PrimaryTeam { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
            public object DeletedAt { get; set; }
            public List<Group> Groups { get; set; }
        }

    }
}