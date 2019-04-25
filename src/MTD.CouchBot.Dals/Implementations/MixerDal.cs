using Microsoft.Extensions.Options;
using MTD.CouchBot.Domain.Models.Bot;
using MTD.CouchBot.Domain.Models.Mixer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace MTD.CouchBot.Dals.Implementations
{
    public class MixerDal : IMixerDal
    {
        private const string BaseUrl = "https://mixer.com/api/v1";
        private readonly BotSettings _botSettings;

        public MixerDal(IOptions<BotSettings> botSettings)
        {
            _botSettings = botSettings.Value;
        }

        public async Task<MixerChannel> GetChannelByName(string name)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create($"{BaseUrl}/channels/{name}");
                request.ContentType = "application/json; charset=utf-8";
                request.Headers.Add("Client-Id", _botSettings.KeySettings.MixerClientId);
                var response = await request.GetResponseAsync();
                var responseText = "";

                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    responseText = sr.ReadToEnd();
                }

                return JsonConvert.DeserializeObject<MixerChannel>(responseText);
            }
            catch(Exception)
            {
                return null;
            }
        }

        public async Task<MixerChannel> GetChannelById(string id)
        {
            return await GetChannelByName(id);
        }

        public async Task<MixerUserResponse> GetUserById(string id)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create($"{BaseUrl}/users/{id}");
                request.ContentType = "application/json; charset=utf-8";
                request.Headers.Add("Client-Id", "_botSettings.KeySettings.MixerClientId");
                var response = await request.GetResponseAsync();
                var responseText = "";

                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    responseText = sr.ReadToEnd();
                }

                return JsonConvert.DeserializeObject<MixerUserResponse>(responseText);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<MixerTeamResponse> GetTeamByToken(string token)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create($"{BaseUrl}/teams/{token}");
                request.ContentType = "application/json; charset=utf-8";
                request.Headers.Add("Client-Id", "_botSettings.KeySettings.MixerClientId");
                var response = await request.GetResponseAsync();
                var responseText = "";

                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    responseText = sr.ReadToEnd();
                }

                return JsonConvert.DeserializeObject<MixerTeamResponse>(responseText);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<MixerTeamResponse> GetTeamById(int id)
        {
            return await GetTeamByToken($"{id}");
        }

        public async Task<List<MixerTeamUserResponse>> GetTeamUsersByTeamId(int teamId)
        {
            var userList = new List<MixerTeamUserResponse>();
            try
            {
                var request = (HttpWebRequest)WebRequest.Create($"{BaseUrl}/teams/{teamId}/users?limit=100");
                request.ContentType = "application/json; charset=utf-8";
                request.Headers.Add("Client-Id", "_botSettings.KeySettings.MixerClientId");
                var response = await request.GetResponseAsync();
                var totalMembers = int.Parse(response.Headers["x-total-count"]);
                var responseText = "";

                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    responseText = sr.ReadToEnd();
                }

                userList.AddRange(JsonConvert.DeserializeObject<List<MixerTeamUserResponse>>(responseText));

                if (totalMembers > 100)
                {
                    var page = 1;

                    do
                    {
                        request = (HttpWebRequest)WebRequest.Create($"{BaseUrl}/teams/{teamId}/users?limit=100&page={page}");
                        request.ContentType = "application/json; charset=utf-8";
                        response = await request.GetResponseAsync();
                        responseText = "";

                        using (var sr = new StreamReader(response.GetResponseStream()))
                        {
                            responseText = sr.ReadToEnd();
                        }

                        userList.AddRange(JsonConvert.DeserializeObject<List<MixerTeamUserResponse>>(responseText));

                        page++;
                    } while (userList.Count < totalMembers);
                }

                return userList;
            }
            catch (Exception)
            {
                Console.WriteLine($"ERROR IN TEAM: {teamId}");
                return null;
            }
        }

        public async Task<List<MixerUserTeamResponse>> GetTeamsByUserId(string id)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create($"{BaseUrl}/users/{id}/teams");
                request.ContentType = "application/json; charset=utf-8";
                request.Headers.Add("Client-Id", "_botSettings.KeySettings.MixerClientId");
                var response = await request.GetResponseAsync();
                var responseText = "";

                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    responseText = sr.ReadToEnd();
                }

                return JsonConvert.DeserializeObject<List<MixerUserTeamResponse>>(responseText);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
