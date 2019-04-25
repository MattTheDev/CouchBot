using MTD.CouchBot.Domain.Enums;

namespace MTD.CouchBot.Domain.Models.Bot
{
    public class RoleCommand
    {
        public string Phrase { get; set; }
        public ulong RoleId { get; set; }
        public RoleFunction Function { get; set; }
    }
}