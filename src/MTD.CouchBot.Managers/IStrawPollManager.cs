using MTD.CouchBot.Domain.Models;
using System.Threading.Tasks;

namespace MTD.CouchBot.Managers
{
    public interface IStrawPollManager
    {
        Task<StrawPoll> CreateStrawPoll(StrawPollRequest poll);
    }
}
