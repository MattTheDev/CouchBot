using MTD.CouchBot.Domain.Models.Piczel;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MTD.CouchBot.Dals.Implementations
{
    public class PiczelDal : IPiczelDal
    {
        public async Task<PiczelStreamResponse> GetStreamById(int id)
        {
            try
            {
                var webRequest = (HttpWebRequest)WebRequest.Create($"https://piczel.tv/api/streams/{id}");
                webRequest.ContentType = "application/json; charset=utf-8";
                string str;

                using (var streamReader = new StreamReader((await webRequest.GetResponseAsync()).GetResponseStream()))
                    str = streamReader.ReadToEnd();

                return JsonConvert.DeserializeObject<PiczelStreamResponse>(str);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<int?> GetUserIdByName(string name)
        {
            try
            {
                var webRequest = (HttpWebRequest)WebRequest.Create($"https://piczel.tv/api/streams/{name}");
                webRequest.ContentType = "application/json; charset=utf-8";
                string str;

                using (var streamReader = new StreamReader((await webRequest.GetResponseAsync()).GetResponseStream()))
                    str = streamReader.ReadToEnd();

                var response = JsonConvert.DeserializeObject<PiczelStreamResponse>(str);

                if(response.Streams.Count == 0)
                {
                    return null;
                }

                return response.Streams.FirstOrDefault(x => x.Username.Equals(name, StringComparison.CurrentCultureIgnoreCase)).Id;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
