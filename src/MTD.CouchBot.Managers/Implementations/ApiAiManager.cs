using MTD.CouchBot.Dals;
using MTD.CouchBot.Dals.Implementations;
using MTD.CouchBot.Domain.Models.Bot;
using System.Threading.Tasks;

namespace MTD.CouchBot.Managers.Implementations
{
    public class ApiAiManager : IApiAiManager
    {
        IApiAiDal _apiAiDal;

        public ApiAiManager()
        {
            _apiAiDal = new ApiAiDal();
        }

        public async Task<ApiAiResponse> AskAQuestion(string question)
        {
            return await _apiAiDal.AskAQuestion(question);
        }
    }
}
