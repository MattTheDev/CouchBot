using Discord.Commands;
using Microsoft.Extensions.Configuration;
using MTD.CouchBot.Localization;
using MTD.CouchBot.Managers;
using MTD.CouchBot.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MTD.CouchBot.Commands
{
    [Group("Message")]
    public class MessageCommands : Command
    {
        private readonly IGroupManager _groupManager;
        private readonly GuildInteractionService _guildInteractionService;

        public MessageCommands(List<Translation> translations, IGuildManager guildManager, IGroupManager groupManager, 
            IConfiguration configuration, GuildInteractionService guildInteractionService) : base(translations, guildManager, groupManager, configuration)
        {
            _groupManager = groupManager;
            _guildInteractionService = guildInteractionService;
        }

        [Command("Live")]
        public async Task Live(string message)
        {
            await Live(message, "Default");
        }

        [Command("Live")]
        public async Task Live(string message, string groupName)
        {
            var translation = await GetTranslation();

            if (!IsOwner)
            {
                return;
            }

            var guildGroup = await _groupManager.GetGuildGroupByGuildIdAndName(Context.Guild.Id, groupName);

            if (guildGroup == null)
            {
                await Context.Channel.SendMessageAsync($"{translation.GroupCommands.InvalidGroupName} '{Prefix} group list'");
                return;
            }

            guildGroup.LiveMessage = message;
            await _groupManager.UpdateGuildGroup(guildGroup);
            await Context.Channel.SendMessageAsync($"{translation.MessageCommands.LiveMessageSet}");
        }

        [Command("Vod")]
        public async Task Vod(string message)
        {
            await Vod(message, "Default");
        }

        [Command("Vod")]
        public async Task Vod(string message, string groupName)
        {
            var translation = await GetTranslation();

            if (!IsOwner)
            {
                return;
            }

            var guildGroup = await _groupManager.GetGuildGroupByGuildIdAndName(Context.Guild.Id, groupName);

            if (guildGroup == null)
            {
                await Context.Channel.SendMessageAsync($"{translation.GroupCommands.InvalidGroupName} '{Prefix} group list'");
                return;
            }

            guildGroup.VodMessage = message;
            await _groupManager.UpdateGuildGroup(guildGroup);
            await Context.Channel.SendMessageAsync($"{translation.MessageCommands.VodMessageSet}");
        }
    }
}
