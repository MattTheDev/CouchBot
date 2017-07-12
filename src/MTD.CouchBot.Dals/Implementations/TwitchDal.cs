using MTD.CouchBot.Domain;
using MTD.CouchBot.Domain.Models.Twitch;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MTD.CouchBot.Dals.Implementations
{
    public class TwitchDal : ITwitchDal
    {
        public async Task<TwitchStreamV5> GetStreamById(string twitchId)
        {
            var request = (HttpWebRequest)WebRequest.Create("https://api.twitch.tv/kraken/streams/" + twitchId);
            request.Headers["Client-Id"] = Constants.TwitchClientId;
            request.Accept = "application/vnd.twitchtv.v5+json";
            var response = await request.GetResponseAsync();
            var responseText = "";

            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                responseText = sr.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<TwitchStreamV5>(responseText);
        }

        public async Task<TwitchStreamsV5> GetStreamsByIdList(string twitchIdList)
        {
            var request = (HttpWebRequest)WebRequest.Create("https://api.twitch.tv/kraken/streams?channel=" + twitchIdList + "&api_version=5");
            request.Headers["Client-Id"] = Constants.TwitchClientId;
            request.Accept = "application/vnd.twitchtv.v5+json";
            var response = await request.GetResponseAsync();
            var responseText = "";

            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                responseText = sr.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<TwitchStreamsV5>(responseText);
        }

        public async Task<string> GetTwitchIdByLogin(string name)
        {
            var request = (HttpWebRequest)WebRequest.Create("https://api.twitch.tv/kraken/users?login=" + name + "&api_version=5");
            request.Headers["Client-Id"] = Constants.TwitchClientId;
            request.Accept = "application/vnd.twitchtv.v5+json";
            var response = await request.GetResponseAsync();
            var responseText = "";

            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                responseText = sr.ReadToEnd();
            }

            var users = JsonConvert.DeserializeObject<TwitchUser>(responseText);

            if(users != null && users.users != null && users.users.Count > 0)
            {
                return users.users[0]._id;
            }
            else
            {
                return null;
            }
        }

        public async Task<TwitchChannelFeed> GetChannelFeedPosts(string twitchId)
        {
            var request = (HttpWebRequest)WebRequest.Create("https://api.twitch.tv/kraken/feed/" + twitchId + "/posts?limit=5&api_version=5");
            request.Headers["Client-Id"] = Constants.TwitchClientId;
            request.Accept = "application/vnd.twitchtv.v5+json";
            var response = await request.GetResponseAsync();
            var responseText = "";

            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                responseText = sr.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<TwitchChannelFeed>(responseText);
        }

        public async Task<TwitchFollowers> GetFollowersByName(string name)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create("https://api.twitch.tv/kraken/channels/" + name + "/follows");
                request.Headers["Client-Id"] = Constants.TwitchClientId;
                var response = await request.GetResponseAsync();
                var responseText = "";

                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    responseText = sr.ReadToEnd();
                }

                return JsonConvert.DeserializeObject<TwitchFollowers>(responseText);
            }
            catch(Exception)
            {
                return null;
            }
        }

        public async Task<TwitchTeam> GetTwitchTeamByName(string name)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create("https://api.twitch.tv/kraken/teams/" + name);
                request.Headers["Client-Id"] = Constants.TwitchClientId;
                request.Accept = "application/vnd.twitchtv.v5+json";
                var response = await request.GetResponseAsync();
                var responseText = "";

                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    responseText = sr.ReadToEnd();
                }

                return JsonConvert.DeserializeObject<TwitchTeam>(responseText);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<string> GetDelimitedListOfTwitchMemberIds(string teamToken)
        {
            var team = await GetTwitchTeamByName(teamToken).ConfigureAwait(false);

            return team == null ? null : string.Join(",", team.Users.Select(u => u.Id));
        }

        public async Task<List<TwitchStreamsV5.Stream>> GetStreamsByGameName(string gameName)
        {
            List<TwitchStreamsV5.Stream> streams = new List<TwitchStreamsV5.Stream>();

            var offset = 0;
            while (true)
            {
                var request = (HttpWebRequest)WebRequest.Create("https://api.twitch.tv/kraken/streams/?game=" + gameName + "&limit=100&stream_type=live&offset=" + offset);
                request.Headers["Client-Id"] = Constants.TwitchClientId;
                request.Accept = "application/vnd.twitchtv.v5+json";
                var response = await request.GetResponseAsync();
                var responseText = "";

                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    responseText = sr.ReadToEnd();
                }

                var streamResponse = JsonConvert.DeserializeObject<TwitchStreamsV5>(responseText);

                if (streamResponse.streams.Count < 1)
                {
                    break;
                }

                streams.AddRange(streamResponse.streams);
                offset += 100;
            }

            return streams;
        }

        public async Task<TwitchGameSearchResponse> SearchForGameByName(string gameName)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create("https://api.twitch.tv/kraken/search/games?query=" + gameName);
                request.Headers["Client-Id"] = Constants.TwitchClientId;
                request.Accept = "application/vnd.twitchtv.v5+json";
                var response = await request.GetResponseAsync();
                var responseText = "";

                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    responseText = sr.ReadToEnd();
                }

                return JsonConvert.DeserializeObject<TwitchGameSearchResponse>(responseText);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
