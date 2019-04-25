using MTD.CouchBot.Domain.Models.Shared;
using System.Collections.Generic;

namespace MTD.CouchBot.Domain.Models.YouTube
{
    public class YouTubeChannelServerModel
    {
        public string YouTubeChannelId { get; set; }
        public bool IsOwner { get; set; }
        public List<ServerOwnerModel> Servers { get; set; }
    }
}
