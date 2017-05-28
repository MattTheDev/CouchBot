using System.Collections.Generic;

namespace MTD.CouchBot.Domain.Models.Mixer
{
    public class MixerChannel
    {
        public class Meta
        {
            public string small { get; set; }
        }

        public class Cover
        {
            public Meta meta { get; set; }
            public int? id { get; set; }
            public string type { get; set; }
            public object relid { get; set; }
            public string url { get; set; }
            public string store { get; set; }
            public string remotePath { get; set; }
            public string createdAt { get; set; }
            public string updatedAt { get; set; }
        }

        public class Type
        {
            public int? id { get; set; }
            public string name { get; set; }
            public string parent { get; set; }
            public string description { get; set; }
            public string source { get; set; }
            public int viewersCurrent { get; set; }
            public string coverUrl { get; set; }
            public int online { get; set; }
        }

        public class Social
        {
            public string twitter { get; set; }
            public string youtube { get; set; }
            public string player { get; set; }
            public List<object> verified { get; set; }
            public string discord { get; set; }
        }

        public class Group
        {
            public int? id { get; set; }
            public string name { get; set; }
        }

        public class Channel
        {
            public int? id { get; set; }
            public int? userId { get; set; }
            public string token { get; set; }
            public bool online { get; set; }
            public bool featured { get; set; }
            public bool partnered { get; set; }
            public int? transcodingProfileId { get; set; }
            public bool suspended { get; set; }
            public string name { get; set; }
            public string audience { get; set; }
            public int viewersTotal { get; set; }
            public int viewersCurrent { get; set; }
            public int numFollowers { get; set; }
            public string description { get; set; }
            public int? typeId { get; set; }
            public bool interactive { get; set; }
            public int? interactiveGameId { get; set; }
            public int ftl { get; set; }
            public bool hasVod { get; set; }
            public object languageId { get; set; }
            public int? coverId { get; set; }
            public object thumbnailId { get; set; }
            public object badgeId { get; set; }
            public object hosteeId { get; set; }
            public bool hasTranscodes { get; set; }
            public bool vodsEnabled { get; set; }
            public string createdAt { get; set; }
            public string updatedAt { get; set; }
            public object deletedAt { get; set; }
        }

        public class User
        {
            public int level { get; set; }
            public Social social { get; set; }
            public int? id { get; set; }
            public string username { get; set; }
            public bool verified { get; set; }
            public int experience { get; set; }
            public int sparks { get; set; }
            public string avatarUrl { get; set; }
            public string bio { get; set; }
            public object primaryTeam { get; set; }
            public string createdAt { get; set; }
            public string updatedAt { get; set; }
            public object deletedAt { get; set; }
            public List<Group> groups { get; set; }
            public Channel channel { get; set; }
        }

        public int? id { get; set; }
        public int? userId { get; set; }
        public string token { get; set; }
        public bool online { get; set; }
        public bool featured { get; set; }
        public bool partnered { get; set; }
        public int? transcodingProfileId { get; set; }
        public bool suspended { get; set; }
        public string name { get; set; }
        public string audience { get; set; }
        public int viewersTotal { get; set; }
        public int viewersCurrent { get; set; }
        public int numFollowers { get; set; }
        public string description { get; set; }
        public int? typeId { get; set; }
        public bool interactive { get; set; }
        public int? interactiveGameId { get; set; }
        public int ftl { get; set; }
        public bool hasVod { get; set; }
        public object languageId { get; set; }
        public int? coverId { get; set; }
        public object thumbnailId { get; set; }
        public object badgeId { get; set; }
        public object hosteeId { get; set; }
        public bool hasTranscodes { get; set; }
        public bool vodsEnabled { get; set; }
        public string createdAt { get; set; }
        public string updatedAt { get; set; }
        public object deletedAt { get; set; }
        public object thumbnail { get; set; }
        public Cover cover { get; set; }
        public object badge { get; set; }
        public Type type { get; set; }
        public User user { get; set; }
    }
}
