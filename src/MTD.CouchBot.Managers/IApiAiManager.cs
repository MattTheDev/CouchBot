using MTD.CouchBot.Domain.Models.Bot;
using System.Threading.Tasks;

namespace MTD.CouchBot.Managers
{
    public interface IApiAiManager
    {
        Task<ApiAiResponse> AskAQuestion(string question);
    }
}
