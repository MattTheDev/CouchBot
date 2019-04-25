using System;

namespace MTD.CouchBot.Domain.Models.Mixer
{
    public class MixerTeamResponse
    {
        public Social social { get; set; }
        public int id { get; set; }
        public int ownerId { get; set; }
        public string token { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string logoUrl { get; set; }
        public object backgroundUrl { get; set; }
        public int totalViewersCurrent { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }

        public class Social
        {
            public string twitter { get; set; }
        }
    }
}