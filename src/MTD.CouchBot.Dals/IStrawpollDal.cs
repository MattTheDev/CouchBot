using MTD.CouchBot.Domain.Models;
using System.Threading.Tasks;

namespace MTD.CouchBot.Dals
{
    public interface IStrawpollDal
    {
        Task<StrawPoll> CreateStrawPoll(StrawPollRequest poll);
    }
}
