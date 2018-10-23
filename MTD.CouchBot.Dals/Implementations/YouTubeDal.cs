using Microsoft.Extensions.Configuration;
using MTD.CouchBot.Domain.Dtos.YouTube;
using MTD.CouchBot.Domain.Exceptions;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MTD.CouchBot.Dals.Implementations
{
    public class YouTubeDal : IYouTubeDal
    {
        private readonly IConfiguration _configuration;
        private readonly string _baseApiUrl = "https://www.googleapis.com/youtube/v3/";

        public YouTubeDal(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<YouTubeChannelQueryByChannelIdResponse> GetYouTubeChannelByChannelId(string channelId)
        {
            if (string.IsNullOrEmpty(_configuration["Keys:YouTubeApiKey"]))
            {
                throw new YouTubeApiKeyMissingException("BotConfig.json is missing your YouTube API Key.");
            }

            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"{_baseApiUrl}channels?part=snippet&id={channelId}&key={_configuration["Keys:YouTubeApiKey"]}");

                    response.EnsureSuccessStatusCode();

                    return JsonConvert.DeserializeObject<YouTubeChannelQueryByChannelIdResponse>(await response.Content.ReadAsStringAsync());
                }
            }
            catch (Exception ex)
            {
                // TODO ERROR LOGGING DAMN YOU
                return null;
            }
        }
    }
}
