using System;
using System.Collections.Generic;
using System.Text;

namespace MTD.CouchBot.Domain.Models.Twitch
{
    public class TwitchChannelServerModel
    {
        public string TwitchChannelId { get; set; }
        public List<ulong> Servers { get; set; }
    }
}
