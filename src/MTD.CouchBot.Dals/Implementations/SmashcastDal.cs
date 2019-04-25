using MTD.CouchBot.Domain.Models.Smashcast;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace MTD.CouchBot.Dals.Implementations
{
    public class SmashcastDal : ISmashcastDal
    {
        public async Task<SmashcastChannel> GetChannelByName(string name)
        {
            var baseUrl = "https://api.smashcast.tv/media/live/";

            var request = (HttpWebRequest)WebRequest.Create(baseUrl + name);
            var response = await request.GetResponseAsync();
            var responseText = "";

            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                responseText = sr.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<SmashcastChannel>(responseText);
        }
    }
}
