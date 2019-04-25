using MTD.CouchBot.Dals;
using MTD.CouchBot.Domain.Models;
using System.Threading.Tasks;

namespace MTD.CouchBot.Managers.Implementations
{
    public class StrawPollManager : IStrawPollManager
    {
        private readonly IStrawpollDal _strawPollDal;

        public StrawPollManager(IStrawpollDal strawPollDal)
        {
            _strawPollDal = strawPollDal;
        }

        public async Task<StrawPoll> CreateStrawPoll(StrawPollRequest poll)
        {
            return await _strawPollDal.CreateStrawPoll(poll);
        }
    }
}
