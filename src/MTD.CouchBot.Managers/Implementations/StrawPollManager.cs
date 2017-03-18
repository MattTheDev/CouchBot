using MTD.CouchBot.Dals;
using MTD.CouchBot.Dals.Implementations;
using MTD.CouchBot.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTD.CouchBot.Managers.Implementations
{
    public class StrawPollManager : IStrawPollManager
    {
        IStrawpollDal strawPollDal;

        public StrawPollManager()
        {
            strawPollDal = new StrawPollDal();
        }

        public async Task<StrawPoll> CreateStrawPoll(StrawPollRequest poll)
        {
            return await strawPollDal.CreateStrawPoll(poll);
        }
    }
}
