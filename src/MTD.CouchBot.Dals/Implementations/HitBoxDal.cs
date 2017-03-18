using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MTD.CouchBot.Domain.Models;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace MTD.CouchBot.Dals.Implementations
{
    public class HitboxDal : IHitboxDal
    {
        public async Task<HitboxChannel> GetChannelByName(string name)
        {
            var baseUrl = "https://api.hitbox.tv/media/live/";

            var request = (HttpWebRequest)WebRequest.Create(baseUrl + name);
            var response = await request.GetResponseAsync();
            var responseText = "";

            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                responseText = sr.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<HitboxChannel>(responseText);
        }
    }
}
