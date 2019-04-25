using MTD.CouchBot.Domain.Models.Shared;
using System.Collections.Generic;

namespace MTD.CouchBot.Domain.Models.Twitch
{
    public class TwitchChannelServerModel
    {
        public string TwitchChannelId { get; set; }
        public bool IsOwner { get; set; }
        public List<ServerOwnerModel> Servers { get; set; }
    }
}
