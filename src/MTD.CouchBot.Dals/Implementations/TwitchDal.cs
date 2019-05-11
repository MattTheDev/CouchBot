using Microsoft.Extensions.Options;
using MTD.CouchBot.Domain.Models.Bot;
using MTD.CouchBot.Domain.Models.Twitch;
using MTD.CouchBot.Domain.Utilities;
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
        private readonly BotSettings _botSettings;

        public TwitchDal(IOptions<BotSettings> botSettings)
        {
            _botSettings = botSettings.Value;
        }

        public async Task<TwitchStreamV5> GetStreamById(string twitchId)
        {
            var request = (HttpWebRequest)WebRequest.Create("https://api.twitch.tv/kraken/streams/" + twitchId);
            request.Headers["Client-Id"] = _botSettings.KeySettings.TwitchClientId;
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
            request.Headers["Client-Id"] = _botSettings.KeySettings.TwitchClientId;
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
            request.Headers["Client-Id"] = _botSettings.KeySettings.TwitchClientId;
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

        public async Task<List<string>> GetTwitchIdsByLoginList(string twitchNameList)
        {
            var userList = new List<string>();

            var url = "https://api.twitch.tv/kraken/users?login=" + twitchNameList + "&api_version=5";
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Headers["Client-Id"] = _botSettings.KeySettings.TwitchClientId;
            request.Accept = "application/vnd.twitchtv.v5+json";
            try
            {
                var response = await request.GetResponseAsync();
                var responseText = "";

                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    responseText = sr.ReadToEnd();
                }

                var users = JsonConvert.DeserializeObject<TwitchUser>(responseText);

                if (users != null && users.users != null && users.users.Count > 0)
                {
                    foreach (var user in users.users)
                    {
                        userList.Add(user._id);
                    }
                }
                else
                {
                    return null;
                }
            }
            catch(Exception ex)
            {
                // TODO MS
                //await _loggingService.LogError("Error in GetTwitchIdsByLoginList. URL: " + url + " - Message: " + ex.Message + " - Stacktrace: + " + ex.StackTrace);
                //return null;
            }

            return userList;
        }

        public async Task<List<TwitchUser.User>> GetTwitchUsersByLoginList(string twitchNameList)
        {
            var url = "https://api.twitch.tv/kraken/users?login=" + twitchNameList + "&api_version=5";
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Headers["Client-Id"] = _botSettings.KeySettings.TwitchClientId;
            request.Accept = "application/vnd.twitchtv.v5+json";
            try
            {
                var response = await request.GetResponseAsync();
                var responseText = "";

                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    responseText = sr.ReadToEnd();
                }

                var users = JsonConvert.DeserializeObject<TwitchUser>(responseText);

                return users.users;
            }
            catch (Exception ex)
            {
                // TODO MS
                //Logging.LogError("Error in GetTwitchIdsByLoginList. URL: " + url + " - Message: " + ex.Message + " - Stacktrace: + " + ex.StackTrace);
                return null;
            }
        }

        public async Task<TwitchFollowers> GetFollowersByName(string name)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create("https://api.twitch.tv/kraken/channels/" + name + "/follows");
                request.Headers["Client-Id"] = _botSettings.KeySettings.TwitchClientId;
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
                request.Headers["Client-Id"] = _botSettings.KeySettings.TwitchClientId;
                request.Accept = "application/vnd.twitchtv.v5+json";
                var response = await request.GetResponseAsync();
                var responseText = "";

                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    responseText = sr.ReadToEnd();
                }

                return JsonConvert.DeserializeObject<TwitchTeam>(responseText);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<string>> GetDelimitedListOfTwitchMemberIds(string teamToken)
        {
            var response = new List<string>();

            try
            {
                var team = await GetTwitchTeamByName(teamToken).ConfigureAwait(false);

                if(team.Users.Count > 100)
                {
                    var idList = new List<string>();

                    for (int i = 0; i < team.Users.Count; i++)
                    {
                        idList.Add(team.Users[i].Id);

                        if(i % 100 == 0)
                        {
                            response.Add(string.Join(",",idList));
                            idList = new List<string>();
                        }
                    }
                }
                else
                {
                    response.Add(string.Join(",", team.Users.Select(u => u.Id)));
                }
            }
            catch (Exception e)
            {
                // TODO MS
                //Logging.LogError($"Error in TwitchDal - GetDelimitedListOfTwitchMemberIds Token {teamToken}: {e.Message} {e.StackTrace}");
            }

            return response;
        }

        public async Task<List<TwitchStreamsV5.Stream>> GetStreamsByGameName(string gameName)
        {
            var streams = new List<TwitchStreamsV5.Stream>();

            var offset = 0;
            while (true)
            {
                var request = (HttpWebRequest)WebRequest.Create("https://api.twitch.tv/kraken/streams/?game=" + gameName + "&limit=100&stream_type=live&offset=" + offset);
                request.Headers["Client-Id"] = _botSettings.KeySettings.TwitchClientId;
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
                request.Headers["Client-Id"] = _botSettings.KeySettings.TwitchClientId;
                request.Accept = "application/vnd.twitchtv.v5+json";
                var response = await request.GetResponseAsync();
                var responseText = "";

                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    responseText = sr.ReadToEnd();
                }

                return JsonConvert.DeserializeObject<TwitchGameSearchResponse>(responseText);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<TwitchChannelResponse> GetTwitchChannelById(string twitchId)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create("https://api.twitch.tv/kraken/channels/" + twitchId + "?api_version=5");
                request.Headers["Client-Id"] = _botSettings.KeySettings.TwitchClientId;
                var response = await request.GetResponseAsync();
                var responseText = "";

                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    responseText = sr.ReadToEnd();
                }

                return JsonConvert.DeserializeObject<TwitchChannelResponse>(responseText);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<TwitchGameQueryResponse> GetGamesByIdList(string idList)
        {
            var request = (HttpWebRequest)WebRequest.Create($"https://api.twitch.tv/helix/games?{idList}");
            request.Headers["Client-Id"] = _botSettings.KeySettings.TwitchClientId;
            request.Accept = "application/vnd.twitchtv.v5+json";
            var response = await request.GetResponseAsync();
            var responseText = "";

            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                responseText = sr.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<TwitchGameQueryResponse>(responseText);
        }

    }
}
