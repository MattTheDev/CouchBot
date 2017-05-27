using MTD.CouchBot.Domain.Models.Picarto;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace MTD.CouchBot.Dals.Implementations
{
    public class PicartoDal : IPicartoDal
    {
        public async Task<PicartoChannel> GetChannelByName(string name)
        {
            var baseUrl = "https://api.picarto.tv/v1/channel/name/";

            var request = (HttpWebRequest)WebRequest.Create(baseUrl + name);
            var response = await request.GetResponseAsync();
            var responseText = "";

            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                responseText = sr.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<PicartoChannel>(responseText);
        }
    }
}
