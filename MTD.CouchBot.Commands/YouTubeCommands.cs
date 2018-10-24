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
    [Group("YouTube")]
    public class YouTubeCommands : BaseCommand
    {
        private readonly IYouTubeManager _youTubeManager;
        private readonly IGroupManager _groupManager;
        private readonly IChannelManager _channelManager;

        public YouTubeCommands(List<Translation> translations, IGuildManager guildManager, IGroupManager groupManager, 
            IConfiguration configuration, IYouTubeManager youTubeManager, IChannelManager channelManager) : base(translations, guildManager, groupManager, configuration)
        {
            _youTubeManager = youTubeManager;
            _groupManager = groupManager;
            _channelManager = channelManager;
        }

        [Command("Lookup")]
        public async Task Lookup(string channelId)
        {
            var stringBuilder = new StringBuilder();
            var response = await _youTubeManager.GetYouTubeChannelByChannelId(channelId);

            if (response?.Items != null && response.Items.Count > 0)
            {
                stringBuilder.AppendLine($"Name: {response.Items[0].Snippet.Title}");

                int viewCount = response.Items[0].Statistics == null ? 0 : int.Parse(response.Items[0].Statistics.ViewCount);

                stringBuilder.AppendLine($"View Count: {viewCount}");
            }
            else
            {
                stringBuilder.AppendLine("Sorry, a YouTube channel with that ID does not exist.");
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
        public async Task Add(string channelId, string groupName)
        {
            var translation = await GetTranslation();

            if (!IsOwner)
            {
                return;
            }

            var youTubeChannel = await _youTubeManager.GetYouTubeChannelByChannelId(channelId);

            if (youTubeChannel == null && youTubeChannel.Items.Count == 0)
            {
                await Context.Channel.SendMessageAsync("Sorry, a YouTube Channel with that ID does not exist.");
                return;
            }

            var group = await _groupManager.GetGuildGroupByGuildIdAndName(Context.Guild.Id, groupName);

            if (group == null)
            {
                await Context.Channel.SendMessageAsync($"{translation.GroupCommands.InvalidGroupName} '{Prefix} group list'");
                return;
            }

            var groupChannel =
                await _channelManager.GetChannelByGuildGroupIdAndChannelId(group.Id, channelId);

            if (groupChannel != null)
            {
                await Context.Channel.SendMessageAsync("Sorry, a this Youtube channel has already been added to this group.");
                return;
            }

            await _channelManager.AddChannel(new GuildGroupChannel
            {
                ChannelId = channelId,
                GuildGroupId = group.Id,
                Platform = Platform.YouTube
            });

            await Context.Channel.SendMessageAsync("This new YouTube channel has been added.");
        }

        [Command("Remove")]
        public async Task Remove(string loginName)
        {
            await Remove(loginName, "Default");
        }

        [Command("Remove")]
        public async Task Remove(string channelId, string groupName)
        {
            var translation = await GetTranslation();

            if (!IsOwner)
            {
                return;
            }

            var youTubeChannel = await _youTubeManager.GetYouTubeChannelByChannelId(channelId);

            if (youTubeChannel == null && youTubeChannel.Items.Count == 0)
            {
                await Context.Channel.SendMessageAsync("Sorry, a YouTube Channel with that name does not exist.");
                return;
            }

            var group = await _groupManager.GetGuildGroupByGuildIdAndName(Context.Guild.Id, groupName);

            if (group == null)
            {
                await Context.Channel.SendMessageAsync($"{translation.GroupCommands.InvalidGroupName} '{Prefix} group list'");
                return;
            }

            var groupChannel =
                await _channelManager.GetChannelByGuildGroupIdAndChannelId(group.Id, channelId);

            if (groupChannel == null)
            {
                await Context.Channel.SendMessageAsync("Sorry, this YouTube Channel isn't assigned to this group.");
                return;
            }

            await _channelManager.RemoveChannel(groupChannel);

            await Context.Channel.SendMessageAsync("This YouTube Channel has been removed.");
        }
    }
}