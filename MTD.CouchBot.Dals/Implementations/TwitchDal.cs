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

        private async Task<TwitchUserQueryResponse> GetTwitchUserQueryResponse(string url)
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

                    return JsonConvert.DeserializeObject<TwitchUserQueryResponse>(await response.Content.ReadAsStringAsync());
                }
            }
            catch (Exception ex)
            {
                // TODO ERROR LOGGING DAMN YOU
                return null;
            }
        }

        public async Task<TwitchUserQueryResponse> GetTwitchUserById(string id)
        {
            return await GetTwitchUserQueryResponse($"{_baseApiUrl}users?id={id}");
        }

        public async Task<TwitchUserQueryResponse> GetTwitchUserByLoginName(string loginName)
        {
            return await GetTwitchUserQueryResponse($"{_baseApiUrl}users?login={loginName}");
        }

        public async Task<TwitchUserQueryResponse> GetTwitchUsersByIdsDelimitedList(string ids)
        {
            return await GetTwitchUserQueryResponse($"{_baseApiUrl}users?{ids}");
        }

        public async Task<TwitchUserQueryResponse> GetTwitchUsersByLoginNameDelimitedList(string loginNames)
        {
            return await GetTwitchUserQueryResponse($"{_baseApiUrl}users?{loginNames}");
        }
    }
}
