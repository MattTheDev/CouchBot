using Discord;
using Discord.Commands;
using Microsoft.Extensions.Configuration;
using MTD.CouchBot.Domain.Dtos.Discord;
using MTD.CouchBot.Domain.Enums;
using MTD.CouchBot.Localization;
using MTD.CouchBot.Managers;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MTD.CouchBot.Commands
{
    [Group("Mixer")]
    public class MixerCommands : BaseCommand
    {
        private readonly IMixerManager _mixerManager;
        private readonly IGroupManager _groupManager;
        private readonly IChannelManager _channelManager;

        public MixerCommands(List<Translation> translations, IGuildManager guildManager, IChannelManager channelManager,
            IGroupManager groupManager, IConfiguration configuration, IMixerManager mixerManager) : base(translations, guildManager, groupManager, configuration)
        {
            _mixerManager = mixerManager;
            _groupManager = groupManager;
            _channelManager = channelManager;
        }

        [Command("Lookup")]
        public async Task Lookup(string channelName)
        {
            var stringBuilder = new StringBuilder();
            var response = await _mixerManager.GetMixerChannelByChannelName(channelName);

            if(response != null)
            {
                stringBuilder.AppendLine($"Name: {response._User.Username}");
                stringBuilder.AppendLine($"View Count: {response.ViewersTotal}");
                stringBuilder.AppendLine($"Follower Count: {response.NumFollowers}");
            }
            else
            {
                stringBuilder.AppendLine("Sorry, a Mixer channel with that name does not exist.");
            }

            var builder = new EmbedBuilder();
            builder.Description = stringBuilder.ToString();

            await Context.Channel.SendMessageAsync("", false, builder.Build());
        }

        [Command("Add")]
        public async Task Add(string loginName)
        {
            await Add(loginName, "Default");
        }

        [Command("Add")]
        public async Task Add(string channelName, string groupName)
        {
            var translation = await GetTranslation();

            if (!IsOwner)
            {
                return;
            }

            var mixerChannel = await _mixerManager.GetMixerChannelByChannelName(channelName);

            if (mixerChannel == null)
            {
                await Context.Channel.SendMessageAsync("Sorry, a Mixer channel with that name does not exist.");
                return;
            }

            var group = await _groupManager.GetGuildGroupByGuildIdAndName(Context.Guild.Id, groupName);

            if (group == null)
            {
                await Context.Channel.SendMessageAsync($"{translation.GroupCommands.InvalidGroupName} '{Prefix} group list'");
                return;
            }

            var groupChannel =
                await _channelManager.GetChannelByGuildGroupIdAndChannelId(group.Id, mixerChannel.Id.ToString());

            if (groupChannel != null)
            {
                await Context.Channel.SendMessageAsync("Sorry, a this Mixer channel has already been added to this group.");
                return;
            }

            await _channelManager.AddChannel(new GuildGroupChannel
            {
                ChannelId = mixerChannel.Id.ToString(),
                GuildGroupId = group.Id,
                Platform = Platform.Mixer
            });

            await Context.Channel.SendMessageAsync("This new Mixer channel has been added.");
        }

        [Command("Remove")]
        public async Task Remove(string loginName)
        {
            await Remove(loginName, "Default");
        }

        [Command("Remove")]
        public async Task Remove(string channelName, string groupName)
        {
            var translation = await GetTranslation();

            if (!IsOwner)
            {
                return;
            }

            var mixerChannel = await _mixerManager.GetMixerChannelByChannelName(channelName);

            if (mixerChannel == null)
            {
                await Context.Channel.SendMessageAsync("Sorry, a Mixer channel with that name does not exist.");
                return;
            }

            var group = await _groupManager.GetGuildGroupByGuildIdAndName(Context.Guild.Id, groupName);

            if (group == null)
            {
                await Context.Channel.SendMessageAsync($"{translation.GroupCommands.InvalidGroupName} '{Prefix} group list'");
                return;
            }

            var groupChannel =
                await _channelManager.GetChannelByGuildGroupIdAndChannelId(group.Id, mixerChannel.Id.ToString());

            if (groupChannel == null)
            {
                await Context.Channel.SendMessageAsync("Sorry, this Mixer channel isn't assigned to this group.");
                return;
            }

            await _channelManager.RemoveChannel(groupChannel);

            await Context.Channel.SendMessageAsync("This Mixer channel has been removed.");
        }
    }
}
