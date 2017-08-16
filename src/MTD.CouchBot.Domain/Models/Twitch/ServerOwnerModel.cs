using System;
using System.Collections.Generic;
using System.Text;

namespace MTD.CouchBot.Domain.Models.Twitch
{
    public class ServerOwnerModel
    {
        public ulong ServerId { get; set; }
        public bool IsOwner { get; set; }
    }
}
