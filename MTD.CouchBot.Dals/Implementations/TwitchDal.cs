using Microsoft.Extensions.Configuration;
using MTD.CouchBot.Domain.Dtos.Twitch;
using MTD.CouchBot.Domain.Exceptions;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MTD.CouchBot.Dals.Implementations
{
    public class TwitchDal : ITwitchDal
    {
        private readonly IConfiguration _configuration;
        private readonly string _baseApiUrl = "https://api.twitch.tv/helix/";

        public TwitchDal(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private async Task<T> GetTwitchQueryResponse<T>(string url) where T : ITwitchQueryResponse
        {
            if (string.IsNullOrEmpty(_configuration["Keys:TwitchClientId"]))
            {
                throw new TwitchClientIdMissingException("BotConfig.json is missing your Twitch Client-Id");
            }

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Client-Id", _configuration["Keys:TwitchClientId"]);
                    var response = await client.GetAsync(url);

                    response.EnsureSuccessStatusCode();

                    return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
                }
            }
            catch (Exception)
            {
                // TODO ERROR LOGGING DAMN YOU
                return default(T);
            }
        }

        public async Task<TwitchUserQueryResponse> GetTwitchUserById(string id)
        {
            return await GetTwitchQueryResponse<TwitchUserQueryResponse>($"{_baseApiUrl}users?id={id}");
        }

        public async Task<TwitchUserQueryResponse> GetTwitchUserByLoginName(string loginName)
        {
            return await GetTwitchQueryResponse<TwitchUserQueryResponse>($"{_baseApiUrl}users?login={loginName}");
        }

        public async Task<TwitchUserQueryResponse> GetTwitchUsersByIdsDelimitedList(string ids)
        {
            return await GetTwitchQueryResponse<TwitchUserQueryResponse>($"{_baseApiUrl}users?{ids}");
        }

        public async Task<TwitchUserQueryResponse> GetTwitchUsersByLoginNameDelimitedList(string loginNames)
        {
            return await GetTwitchQueryResponse<TwitchUserQueryResponse>($"{_baseApiUrl}users?{loginNames}");
        }

        public async Task<TwitchStreamQueryResponse> GetTwitchStreamByUserId(string id)
        {
            return await GetTwitchQueryResponse<TwitchStreamQueryResponse>($"{_baseApiUrl}streams?user_id={id}");
        }

        public async Task<TwitchStreamQueryResponse> GetTwitchStreamsByUserIdsDelimitedList(string ids)
        {
            return await GetTwitchQueryResponse<TwitchStreamQueryResponse>($"{_baseApiUrl}streams?{ids}");
        }

        public async Task<TwitchGameQueryResponse> GetTwitchGamesByIdsDelimitedList(string ids)
        {
            return await GetTwitchQueryResponse<TwitchGameQueryResponse>($"{_baseApiUrl}games?id={ids}");
        }
    }
}
