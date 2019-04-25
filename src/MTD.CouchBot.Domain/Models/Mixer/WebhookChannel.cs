using System;

namespace MTD.CouchBot.Domain.Models.Mixer
{
    public class WebhookChannel
    {
        public uint Id { get; set; }
        public uint UserId { get; set; }
        public string Token { get; set; }
        public bool Online { get; set; }
        public bool Featured { get; set; }
        public int FeatureLevel { get; set; }
        public bool Partnered { get; set; }
        public uint? TranscodingProfileId { get; set; }
        public bool Suspended { get; set; }
        public string Name { get; set; }
        public string Audience { get; set; }
        public uint ViewersTotal { get; set; }
        public uint ViewersCurrent { get; set; }
        public uint NumFollowers { get; set; }
        public string Description { get; set; }
        public uint? TypeId { get; set; }
        public bool Interactive { get; set; }
        public uint? InteractiveGameId { get; set; }
        public uint Ftl { get; set; }
        public bool HasVod { get; set; }
        public string LanguageId { get; set; }
        public uint? CoverId { get; set; }
        public uint? ThumbnailId { get; set; }
        public uint BadgeId { get; set; }
        public string BannerUrl { get; set; }
        public uint HosteeId { get; set; }
        public bool HasTranscodes { get; set; }
        public bool VodsEnabled { get; set; }
        public Guid? CostreamId { get; set; }
    }
}