using System.Collections.Generic;

namespace MTD.CouchBot.Domain.Models.DLive
{
    public class ChannelQueryResponse
    {
        public Data data { get; set; }

        public class Followers
        {
            public int totalCount { get; set; }
            public string __typename { get; set; }
        }

        public class Category
        {
            public string title { get; set; }
            public string imgUrl { get; set; }
            public string id { get; set; }
            public int backendID { get; set; }
            public string __typename { get; set; }
        }

        public class Creator
        {
            public string username { get; set; }
            public string __typename { get; set; }
        }

        public class Language
        {
            public string language { get; set; }
            public string __typename { get; set; }
        }

        public class Livestream
        {
            public Category category { get; set; }
            public string title { get; set; }
            public int watchingCount { get; set; }
            public string totalReward { get; set; }
            public string permlink { get; set; }
            public Creator creator { get; set; }
            public string __typename { get; set; }
            public string content { get; set; }
            public string id { get; set; }
            public bool watchTime { get; set; }
            public bool disableAlert { get; set; }
            public Language language { get; set; }
        }

        public class TreasureChest
        {
            public string value { get; set; }
            public string state { get; set; }
            public object ongoingGiveaway { get; set; }
            public string __typename { get; set; }
            public string expireAt { get; set; }
            public List<object> buffs { get; set; }
            public string startGiveawayValueThreshold { get; set; }
        }

        public class Videos
        {
            public int totalCount { get; set; }
            public string __typename { get; set; }
        }

        public class PastBroadcasts
        {
            public int totalCount { get; set; }
            public string __typename { get; set; }
        }

        public class Following
        {
            public int totalCount { get; set; }
            public string __typename { get; set; }
        }

        public class UserByDisplayName
        {
            public string id { get; set; }
            public string avatar { get; set; }
            public string __typename { get; set; }
            public string displayname { get; set; }
            public string partnerStatus { get; set; }
            public string username { get; set; }
            public Followers followers { get; set; }
            public bool canSubscribe { get; set; }
            public object subSetting { get; set; }
            public string offlineImage { get; set; }
            public string banStatus { get; set; }
            public bool deactivated { get; set; }
            public string about { get; set; }
            public Livestream livestream { get; set; }
            public TreasureChest treasureChest { get; set; }
            public object hostingLivestream { get; set; }
            public Videos videos { get; set; }
            public PastBroadcasts pastBroadcasts { get; set; }
            public Following following { get; set; }
        }

        public class Data
        {
            public UserByDisplayName userByDisplayName { get; set; }
        }
    }
}