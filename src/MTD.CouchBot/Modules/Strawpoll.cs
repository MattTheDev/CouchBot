using Discord;
using Discord.Commands;
using MTD.CouchBot.Domain.Models;
using MTD.CouchBot.Json;
using MTD.CouchBot.Managers;
using MTD.CouchBot.Managers.Implementations;
using MTD.CouchBot.Domain.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MTD.CouchBot.Modules
{
    [Group("strawpoll")]
    public class StrawPoll : ModuleBase
    {
        IStrawPollManager strawPollManager;

        public StrawPoll()
        {
            strawPollManager = new StrawPollManager();
        }

        [Command("create"), Summary("Create a StrawPoll.")]
        public async Task Create(string pollArgs)
        {
            StrawPollRequest poll = new StrawPollRequest();

            var args = pollArgs.Split('|');
            poll.title = args[0];
            poll.options = new List<string>();

            foreach(var o in args[1].Split(','))
            {
                poll.options.Add(o);
            }

            poll.multi = bool.Parse(args[2]);

            var newPoll = await strawPollManager.CreateStrawPoll(poll);

            await Context.Channel.SendMessageAsync("Poll Created: http://www.strawpoll.me/" + newPoll.id);
        }
    }

}
