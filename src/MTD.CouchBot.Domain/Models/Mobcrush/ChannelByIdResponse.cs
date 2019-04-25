namespace MTD.CouchBot.Domain.Models.Mobcrush
{
    public class ChannelByIdResponse
    {
        public string _id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public int memberCount { get; set; }
        public string chatroomObjectId { get; set; }
    }
}
