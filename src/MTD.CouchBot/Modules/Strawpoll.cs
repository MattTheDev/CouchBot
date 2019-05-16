using Discord.Commands;
using MTD.CouchBot.Domain.Models;
using MTD.CouchBot.Managers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MTD.CouchBot.Modules
{
    [Group("strawpoll")]
    public class StrawPoll : ModuleBase
    {
        private readonly StrawPollManager _strawPollManager;

        public StrawPoll(StrawPollManager strawPollManager)
        {
            _strawPollManager = strawPollManager;
        }

        [Command("create"), Summary("Create a StrawPoll.")]
        public async Task Create(string pollArgs)
        {
            var poll = new StrawPollRequest();

            var args = pollArgs.Split('|');
            poll.title = args[0];
            poll.options = new List<string>();

            foreach(var o in args[1].Split(','))
            {
                poll.options.Add(o);
            }

            poll.multi = bool.Parse(args[2]);

            var newPoll = await _strawPollManager.CreateStrawPoll(poll);

            await Context.Channel.SendMessageAsync("Poll Created: http://www.strawpoll.me/" + newPoll.id);
        }
    }

}
