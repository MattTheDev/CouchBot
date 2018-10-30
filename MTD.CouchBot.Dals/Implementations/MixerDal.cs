using Microsoft.Extensions.Configuration;
using MTD.CouchBot.Domain.Dtos.Mixer;
using MTD.CouchBot.Domain.Exceptions;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MTD.CouchBot.Dals.Implementations
{
    public class MixerDal : IMixerDal
    {
        private readonly IConfiguration _configuration;
        private readonly string _baseApiUrl = "https://mixer.com/api/v1/";

        public MixerDal(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<MixerChannelQueryResponse> GetMixerChannelByChannelName(string channelName)
        {
            if (string.IsNullOrEmpty(_configuration["Keys:MixerClientId"]))
            {
                throw new MixerClientIdMissingException("BotConfig.json is missing your Mixer Client-Id");
            }
            
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Client-Id", _configuration["Keys:MixerClientId"]);
                    var response = await client.GetAsync($"{_baseApiUrl}channels/{channelName}");

                    response.EnsureSuccessStatusCode();

                    return JsonConvert.DeserializeObject<MixerChannelQueryResponse>(await response.Content.ReadAsStringAsync());
                }
            }
            catch (Exception)
            {
                // TODO ERROR LOGGING DAMN YOU
                return null;
            }
        }
    }
}