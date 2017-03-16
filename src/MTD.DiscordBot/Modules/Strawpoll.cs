using Discord;
using Discord.Commands;
using MTD.DiscordBot.Domain.Models;
using MTD.DiscordBot.Json;
using MTD.DiscordBot.Managers;
using MTD.DiscordBot.Managers.Implementations;
using MTD.DiscordBot.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MTD.DiscordBot.Modules
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
