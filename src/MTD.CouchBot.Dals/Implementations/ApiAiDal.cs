using MTD.CouchBot.Domain.Models.Bot;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace MTD.CouchBot.Dals.Implementations
{
    public class ApiAiDal : IApiAiDal
    {
        public async Task<ApiAiResponse> AskAQuestion(string question)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create("https://api.api.ai/v1/query?query=How are you?&lang=en&sessionId=CouchBot");
            webRequest.Headers["Authorization"] = "Bearer 1fc5f67804ed4fef9a7388bfde1b4948";
            webRequest.ContentType = "application/json; charset=utf-8";
            string str;
            using (StreamReader streamReader = new StreamReader((await webRequest.GetResponseAsync()).GetResponseStream()))
                str = streamReader.ReadToEnd();

            return JsonConvert.DeserializeObject<ApiAiResponse>(str);
        }
    }
}
