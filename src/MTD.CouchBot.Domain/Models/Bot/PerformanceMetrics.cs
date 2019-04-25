using System;

namespace MTD.CouchBot.Domain.Models.Bot
{
    public class PerformanceMetrics
    {
        public int Id { get; set; }
        public string Platform { get; set; }
        public bool IsOwner { get; set; }
        public long RunTime { get; set; }
        public long ScheduledInterval { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
