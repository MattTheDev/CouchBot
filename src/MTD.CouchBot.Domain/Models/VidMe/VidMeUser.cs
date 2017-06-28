using System;
using System.Collections.Generic;
using System.Text;

namespace MTD.CouchBot.Domain.Models.VidMe
{
    public class VidMeUser
    {
        public bool status { get; set; }
        public User user { get; set; }
        public Watchers watchers { get; set; }

        public class User
        {
            public string user_id { get; set; }
            public string username { get; set; }
            public string full_url { get; set; }
            public string avatar { get; set; }
            public string avatar_url { get; set; }
            public string avatar_ai { get; set; }
            public string cover { get; set; }
            public string cover_url { get; set; }
            public string cover_ai { get; set; }
            public string displayname { get; set; }
            public int follower_count { get; set; }
            public string likes_count { get; set; }
            public int video_count { get; set; }
            public int watching_count { get; set; }
            public string video_views { get; set; }
            public int videos_scores { get; set; }
            public int comments_scores { get; set; }
            public string bio { get; set; }
            public bool disallow_downloads { get; set; }
        }

        public class Watchers
        {
            public int total { get; set; }
            public List<string> countries { get; set; }
        }
    }
}
