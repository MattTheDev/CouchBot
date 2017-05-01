using System;

namespace MTD.CouchBot.Json
{
    public class User
    {
        public ulong Id { get; set; }
        public DateTime? Birthday { get; set; }
        public float TimeZoneOffset { get; set; }
    }
}
