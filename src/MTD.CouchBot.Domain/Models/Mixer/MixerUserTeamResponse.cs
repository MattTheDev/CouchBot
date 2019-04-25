using System;
using System.Collections.Generic;

namespace MTD.CouchBot.Domain.Models.Mixer
{
    public class MixerUserTeamResponse
    {
        public class Social
        {
            public string twitter { get; set; }
        }

        public class Social2
        {
            public string youtube { get; set; }
            public string twitter { get; set; }
            public string player { get; set; }
            public List<object> verified { get; set; }
            public string instagram { get; set; }
            public string steam { get; set; }
            public string patreon { get; set; }
        }

        public class Owner
        {
            public int level { get; set; }
            public Social2 social { get; set; }
            public int id { get; set; }
            public string username { get; set; }
            public bool verified { get; set; }
            public int? experience { get; set; }
            public int? sparks { get; set; }
            public string avatarUrl { get; set; }
            public string bio { get; set; }
            public int? primaryTeam { get; set; }
            public DateTime createdAt { get; set; }
            public DateTime updatedAt { get; set; }
            public object deletedAt { get; set; }
        }

        public class TeamMembership
        {
            public int teamId { get; set; }
            public int userId { get; set; }
            public bool accepted { get; set; }
            public DateTime createdAt { get; set; }
            public DateTime updatedAt { get; set; }
        }

        public Social social { get; set; }
        public int id { get; set; }
        public int ownerId { get; set; }
        public string token { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string logoUrl { get; set; }
        public string backgroundUrl { get; set; }
        public int totalViewersCurrent { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public Owner owner { get; set; }
        public TeamMembership teamMembership { get; set; }
        public bool isPrimary { get; set; }

    }
}