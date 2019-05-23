using System.Threading.Tasks;
using MTD.CouchBot.Dals;
using MTD.CouchBot.Domain.Models;

namespace MTD.CouchBot.Managers
{
    public class StrawPollManager
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
