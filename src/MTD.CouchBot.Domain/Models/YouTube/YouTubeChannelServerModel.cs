using System;
using System.Collections.Generic;
using System.Text;

namespace MTD.CouchBot.Domain.Models.YouTube
{
    public class YouTubeChannelServerModel
    {
        public string YouTubeChannelId { get; set; }
        public List<ulong> Servers { get; set; }
    }
}
