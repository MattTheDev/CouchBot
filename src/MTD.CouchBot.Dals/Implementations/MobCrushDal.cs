using MTD.CouchBot.Domain.Models.Mobcrush;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MTD.CouchBot.Dals.Implementations
{
    public class MobcrushDal : IMobcrushDal
    {
        public async Task<ChannelBroadcastResponse> GetMobcrushBroadcastByChannelId(string id)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create("https://api.mobcrush.com/channelBroadcasts/" + id);
                var response = await request.GetResponseAsync();
                var responseText = "";

                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    responseText = sr.ReadToEnd();
                }

                var streams = JsonConvert.DeserializeObject<List<ChannelBroadcastResponse>>(responseText);

                return streams.FirstOrDefault(s => s.IsLive);
            }
            catch(Exception)
            {
                return null;
            }
        }

        public async Task<ChannelResponse> GetMobcrushIdByName(string name)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create("https://www.mobcrush.com/api/channel/web/" + name);
                var response = await request.GetResponseAsync();
                var responseText = "";

                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    responseText = sr.ReadToEnd();
                }

                return JsonConvert.DeserializeObject<ChannelResponse>(responseText);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<ChannelByIdResponse> GetMobcrushChannelById(string id)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create("https://www.mobcrush.com/api/channel/" + id);
                var response = await request.GetResponseAsync();
                var responseText = "";

                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    responseText = sr.ReadToEnd();
                }

                return JsonConvert.DeserializeObject<ChannelByIdResponse>(responseText);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<UserResponse> GetMobcrushStreamById(string id)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create("https://www.mobcrush.com/api/user/" + id);
                var response = await request.GetResponseAsync();
                var responseText = "";

                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    responseText = sr.ReadToEnd();
                }

                return JsonConvert.DeserializeObject<UserResponse>(responseText);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
