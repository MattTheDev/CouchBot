using System.Collections.Generic;

namespace MTD.CouchBot.Domain.Models.Bot
{
    public class Admin
    {
        public List<ulong> Users { get; set; }
        public List<ulong> Roles { get; set; }

    }
}