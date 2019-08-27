namespace MTD.CouchBot.Domain.Models.Bot
{
    public class XPromo
    {
        public ulong ChannelId { get; set; }
        public bool AllowMixer { get; set; }
        public bool AllowMobcrush { get; set; }
        public bool AllowPicarto { get; set; }
        public bool AllowPiczel { get; set; }
        public bool AllowSmashcast { get; set; }
        public bool AllowTwitch { get; set; }
        public bool AllowYouTube { get; set; }
    }
}