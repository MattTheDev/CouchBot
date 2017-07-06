using MTD.CouchBot.Dals;
using MTD.CouchBot.Dals.Implementations;
using MTD.CouchBot.Domain.Models;
using System.Threading.Tasks;

namespace MTD.CouchBot.Managers.Implementations
{
    public class StrawPollManager : IStrawPollManager
    {
        IStrawpollDal _strawPollDal;

        public StrawPollManager()
        {
            _strawPollDal = new StrawPollDal();
        }

        public async Task<StrawPoll> CreateStrawPoll(StrawPollRequest poll)
        {
            return await _strawPollDal.CreateStrawPoll(poll);
        }
    }
}
