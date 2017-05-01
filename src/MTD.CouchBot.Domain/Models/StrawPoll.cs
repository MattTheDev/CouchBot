using System.Collections.Generic;

namespace MTD.CouchBot.Domain.Models
{
    public class StrawPoll
    {
        public int id { get; set; }
        public string title { get; set; }
        public List<string> options { get; set; }
        public List<int> votes { get; set; }
        public bool multi { get; set; }
        public string dupcheck { get; set; }
        public bool captcha { get; set; }
    }
}
