using MTD.CouchBot.Domain.Models.Mixer;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace MTD.CouchBot.Dals.Implementations
{
    public class MixerDal : IMixerDal
    {
        public async Task<MixerChannel> GetChannelByName(string name)
        {
            var baseUrl = "https://mixer.com/api/v1";

            var request = (HttpWebRequest)WebRequest.Create(baseUrl + "/channels/" + name);
            request.ContentType = "application/json; charset=utf-8";
            var response = await request.GetResponseAsync();
            var responseText = "";

            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                responseText = sr.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<MixerChannel>(responseText);
        }

        public async Task<MixerChannel> GetChannelById(string id)
        {
            return await GetChannelByName(id);
        }
    }
}
