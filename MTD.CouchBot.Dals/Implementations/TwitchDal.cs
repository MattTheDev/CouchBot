using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MTD.CouchBot.Domain.Dtos.Twitch;
using MTD.CouchBot.Domain.Exceptions;
using Newtonsoft.Json;

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

        public async Task<TwitchUserQueryResponse> GetTwitchUserByLoginName(string loginName)
        {
            if(string.IsNullOrEmpty(_configuration["Keys:TwitchClientId"]))
            {
                throw new TwitchClientIdMissingException("BotConfig.json is missing your Twitch Client-Id");
            }

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Client-Id", _configuration["Keys:TwitchClientId"]);
                    var response = await client.GetAsync($"{_baseApiUrl}users?login={loginName}");

                    response.EnsureSuccessStatusCode();

                    return JsonConvert.DeserializeObject<TwitchUserQueryResponse>(await response.Content.ReadAsStringAsync());
                }
            }
            catch(Exception ex)
            {
                // TODO ERROR LOGGING DAMN YOU
                return null;
            }
        }
    }
}
