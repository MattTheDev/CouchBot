using MTD.CouchBot.Domain;
using MTD.CouchBot.Domain.Models;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace MTD.CouchBot.Dals.Implementations
{
    public class TwitchDal : ITwitchDal
    {
        public async Task<TwitchStreamV5> GetStreamById(string twitchId)
        {
            var request = (HttpWebRequest)WebRequest.Create("https://api.twitch.tv/kraken/streams/" + twitchId);
            request.Headers["Client-Id"] = Constants.TwitchClientId;
            request.Accept = "application/vnd.twitchtv.v5+json";
            var response = await request.GetResponseAsync();
            var responseText = "";

            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                responseText = sr.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<TwitchStreamV5>(responseText);
        }

        public async Task<TwitchStreamsV5> GetStreamsByIdList(string twitchIdList)
        {
            var request = (HttpWebRequest)WebRequest.Create("https://api.twitch.tv/kraken/streams?channel=" + twitchIdList + "&api_version=5");
            request.Headers["Client-Id"] = Constants.TwitchClientId;
            request.Accept = "application/vnd.twitchtv.v5+json";
            var response = await request.GetResponseAsync();
            var responseText = "";

            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                responseText = sr.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<TwitchStreamsV5>(responseText);
        }

        public async Task<string> GetTwitchIdByLogin(string name)
        {
            var request = (HttpWebRequest)WebRequest.Create("https://api.twitch.tv/kraken/users?login=" + name + "&api_version=5");
            request.Headers["Client-Id"] = Constants.TwitchClientId;
            request.Accept = "application/vnd.twitchtv.v5+json";
            var response = await request.GetResponseAsync();
            var responseText = "";

            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                responseText = sr.ReadToEnd();
            }

            var users = JsonConvert.DeserializeObject<TwitchUser>(responseText);

            if(users != null && users.users != null && users.users.Count > 0)
            {
                return users.users[0]._id;
            }
            else
            {
                return null;
            }
        }
        
        // TODO: Implement followers by ID <<----
        public async Task<TwitchFollowers> GetFollowersByName(string name)
        {
            var request = (HttpWebRequest)WebRequest.Create("https://api.twitch.tv/kraken/channels/" + name + "/follows");
            request.Headers["Client-Id"] = Constants.TwitchClientId;
            var response = await request.GetResponseAsync();
            var responseText = "";

            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                responseText = sr.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<TwitchFollowers>(responseText);
        }
    }
}
