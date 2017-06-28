using MTD.CouchBot.Domain.Models.VidMe;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System;
using System.Net.Http;

namespace MTD.CouchBot.Dals.Implementations
{
    public class VidMeDal : IVidMeDal
    {
        public async Task<VidMeUserVideos> GetUserVideosById(int id)
        {
            var url = string.Format("https://api.vid.me/user/{0}/videos", id);

            var request = (HttpWebRequest)WebRequest.Create(url);
            var response = await request.GetResponseAsync();
            var responseText = "";

            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                responseText = sr.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<VidMeUserVideos>(responseText);
        }

        public async Task<VidMeUser> GetUserById(int id)
        {
            var url = string.Format("https://api.vid.me/user/{0}", id);

            var request = (HttpWebRequest)WebRequest.Create(url);
            var response = await request.GetResponseAsync();
            var responseText = "";

            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                responseText = sr.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<VidMeUser>(responseText);
        }

        public async Task<int> GetIdByName(string name)
        {
            HttpClient client = new HttpClient();

            try
            {
                var response = await client.GetStringAsync("https://vid.me/" + name);
                var start = response.IndexOf("data-user=\"");
                var tempStr = response.Substring(start + 11);
                var end = tempStr.IndexOf("\"");
                var id = tempStr.Substring(0, end);

                return Int32.Parse(id);
            }
            catch(Exception ex)
            {
                return 0;
            }
        }
    }
}
