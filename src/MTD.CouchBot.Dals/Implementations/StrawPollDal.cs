using MTD.CouchBot.Domain.Models;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MTD.CouchBot.Dals.Implementations
{
    public class StrawPollDal : IStrawpollDal
    {
        public async Task<StrawPoll> CreateStrawPoll(StrawPollRequest poll)
        {
            var fullQuery = "https://www.strawpoll.me/api/v2/polls";

            var request = WebRequest.Create(fullQuery);
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";

            var postString = JsonConvert.SerializeObject(poll);
            var data = Encoding.UTF8.GetBytes(postString);
            var newStream = await request.GetRequestStreamAsync();
            newStream.Write(data, 0, data.Length);
            newStream.Dispose();

            var response = await request.GetResponseAsync();
            var requestReader = new StreamReader(response.GetResponseStream());
            var webResponse = requestReader.ReadToEnd();
            response.Dispose();

            return JsonConvert.DeserializeObject<StrawPoll>(webResponse);
        }
    }
}
