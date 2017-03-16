using MTD.DiscordBot.Dals;
using MTD.DiscordBot.Dals.Implementations;
using MTD.DiscordBot.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTD.DiscordBot.Managers.Implementations
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
