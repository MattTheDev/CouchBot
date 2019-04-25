using System;
using System.Collections.Generic;

namespace MTD.CouchBot.Domain.Models.Mixer
{
    public class MixerTeamUserResponse
    {
        public int level { get; set; }
        public Social social { get; set; }
        public int id { get; set; }
        public string username { get; set; }
        public bool verified { get; set; }
        public int experience { get; set; }
        public int sparks { get; set; }
        public string avatarUrl { get; set; }
        public string bio { get; set; }
        public int? primaryTeam { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public object deletedAt { get; set; }
        public MixerChannel channel { get; set; }
        public TeamMembership teamMembership { get; set; }

        public class Social
        {
            public List<object> verified { get; set; }
            public string twitter { get; set; }
            public string youtube { get; set; }
            public string facebook { get; set; }
            public string player { get; set; }
            public string instagram { get; set; }
            public string steam { get; set; }
        }
        
        public class TeamMembership
        {
            public int teamId { get; set; }
            public int userId { get; set; }
            public bool accepted { get; set; }
            public DateTime createdAt { get; set; }
            public DateTime updatedAt { get; set; }
        }
    }
}