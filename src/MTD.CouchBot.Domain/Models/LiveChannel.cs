using System.Collections.Generic;

namespace MTD.CouchBot.Domain.Models
{
    public class LiveChannel
    {
        public string Name { get; set; }
        public List<ulong> Servers { get; set; }
        public List<ChannelMessage> ChannelMessages {get;set;}
    }
}
