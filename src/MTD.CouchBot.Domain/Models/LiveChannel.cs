using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTD.CouchBot.Domain.Models
{
    public class LiveChannel
    {
        public string Name { get; set; }
        public List<ulong> Servers { get; set; }
        public List<ChannelMessage> ChannelMessages {get;set;}
    }
}
