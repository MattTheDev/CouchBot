using System.Collections.Generic;

namespace MTD.CouchBot.Domain.Models
{
    public class TwitchFollowers
    {
        public class Links
        {
            public string self { get; set; }
        }

        public class Links2
        {
            public string self { get; set; }
        }

        public class User
        {
            public int _id { get; set; }
            public string name { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
            public Links2 _links { get; set; }
            public string display_name { get; set; }
            public string logo { get; set; }
            public string bio { get; set; }
            public string type { get; set; }
        }

        public class Follow
        {
            public string created_at { get; set; }
            public Links _links { get; set; }
            public bool notifications { get; set; }
            public User user { get; set; }
        }

        public class Links3
        {
            public string self { get; set; }
            public string next { get; set; }
        }

        public List<Follow> follows { get; set; }
        public int _total { get; set; }
        public Links3 _links { get; set; }
        public string _cursor { get; set; }
    }
}
