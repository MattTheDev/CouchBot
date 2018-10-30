using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MTD.CouchBot.Domain.Dtos.Twitch
{
    public class TwitchStreamQueryResponse
    {
        [JsonProperty("data")]
        public List<Stream> Streams { get; set; }
        public Pagination _Pagination { get; set; }

        public class Stream
        {
            public string Id { get; set; }
            [JsonProperty("user_id")]
            public string UserId { get; set; }
            [JsonProperty("user_name")]
            public string Username { get; set; }
            [JsonProperty("game_id")]
            public string GameId { get; set; }
            [JsonProperty("community_ids")]
            public List<string> Tags { get; set; }
            public string Type { get; set; }
            public string Title { get; set; }
            [JsonProperty("viewer_count")]
            public int ViewerCount { get; set; }
            [JsonProperty("started_at")]
            public DateTime StartedAt { get; set; }
            public string Language { get; set; }
            [JsonProperty("thumbnail_url")]
            public string ThumbnailUrl { get; set; }
        }

        public class Pagination
        {
            public string Cursor { get; set; }
        }
    }
}
