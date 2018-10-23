using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Configuration;
using MTD.CouchBot.Domain.Enums;
using MTD.CouchBot.Domain.Utilities;
using MTD.CouchBot.Localization;
using MTD.CouchBot.Managers;

namespace MTD.CouchBot.Commands
{
    [Group("Channel")]
    public class ChannelCommands : Command
    {
        private readonly IGroupManager _groupManager;

        public ChannelCommands(List<Translation> translations, IGuildManager guildManager, IGroupManager groupManager, IConfiguration configuration, IGroupManager groupManager1) : base(translations, guildManager, groupManager, configuration)
        {
            _groupManager = groupManager1;
        }

        [Command("Live")]
        public async Task Live(IChannel channel)
        {
            await Live(channel, "Default");
        }

        [Command("Live")]
        public async Task Live(IChannel channel, string groupName)
        {
            var translation = await GetTranslation();

            if (!IsOwner)
            {
                return;
            }

            var group = await _groupManager.GetGuildGroupByGuildIdAndName(Context.Guild.Id, groupName);

            if (group == null)
            {
                await Context.Channel.SendMessageAsync($"{translation.GroupCommands.InvalidGroupName} '{Prefix} group list'");
                return;
            }

            group.StreamChannelId = Cryptography.Encrypt(channel.Id.ToString());

            await _groupManager.UpdateGuildGroup(group);

            await Context.Channel.SendMessageAsync($"{translation.ChannelCommands.LiveChannelSet}");
        }

        [Command("Vod")]
        public async Task Vod(IChannel channel)
        {
            await Vod(channel, "Default");
        }

        [Command("Vod")]
        public async Task Vod(IChannel channel, string groupName)
        {
            var translation = await GetTranslation();

            if (!IsOwner)
            {
                return;
            }

            var group = await _groupManager.GetGuildGroupByGuildIdAndName(Context.Guild.Id, groupName);

            if (group == null)
            {
                await Context.Channel.SendMessageAsync($"{translation.GroupCommands.InvalidGroupName} '{Prefix} group list'");
                return;
            }

            group.VodChannelId = Cryptography.Encrypt(channel.Id.ToString());

            await _groupManager.UpdateGuildGroup(group);

            await Context.Channel.SendMessageAsync($"{translation.ChannelCommands.VodChannelSet}");
        }
    }
}