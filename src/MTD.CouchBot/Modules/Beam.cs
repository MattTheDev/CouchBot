using Discord.Commands;
using MTD.CouchBot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTD.DiscordBot.Modules
{
    [Group("beam")]
    public class Beam : ModuleBase
    {
        [Command("status")]
        public async Task Status()
        {
            await Context.Channel.SendMessageAsync("Current CouchBot Beam Constellation Connection Status: " + Program.beamClient.Status());
        }
    }
}
