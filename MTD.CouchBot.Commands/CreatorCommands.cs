using Discord;
using Discord.Commands;
using Microsoft.Extensions.Configuration;
using MTD.CouchBot.Domain.Dtos.Twitch;
using MTD.CouchBot.Domain.Enums;
using MTD.CouchBot.Localization;
using MTD.CouchBot.Managers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTD.CouchBot.Commands
{
    [Group("Creator"), Alias("C")]
    public class CreatorCommands : Command
    {
        private readonly IGroupManager _groupManager;
        private readonly IChannelManager _channelManager;
        private readonly ITwitchManager _twitchManager;

        public CreatorCommands(List<Translation> translations, IGuildManager guildManager, IGroupManager groupManager, 
            IConfiguration configuration, IChannelManager channelManager, ITwitchManager twitchManager) : base(translations, guildManager, groupManager, configuration)
        {
            _groupManager = groupManager;
            _channelManager = channelManager;
            _twitchManager = twitchManager;
        }

        [Command("List")]
        public async Task List()
        {
            // TODO - This should prompt user to provide a platform. Do not display actual creators.
        }

        [Command("List")]
        public async Task List(Platform platform)
        {
            await List(platform, "Default");
        }

        [Command("List")]
        public async Task List(Platform platform, string groupName)
        {
            var translation = await GetTranslation();

            var guildGroup = await _groupManager.GetGuildGroupByGuildIdAndName(Context.Guild.Id, groupName);

            if (guildGroup == null)
            {
                await Context.Channel.SendMessageAsync($"{translation.GroupCommands.InvalidGroupName} '{Prefix} group list'");
            }

            var guildGroupChannels = await _channelManager.GetChannelsByGuildGroupIdAndPlatform(guildGroup.Id, platform);

            var builder = new EmbedBuilder();
            builder.Description = $"Your list of {platform} creators in the {groupName} group";

            if (guildGroupChannels != null && guildGroupChannels.Count > 0)
            {
                var twitchUsers = new TwitchUserQueryResponse();
                if (guildGroupChannels.Count == 1)
                {
                    twitchUsers =
                        await _twitchManager.GetTwitchUserById(guildGroupChannels[0].ChannelId);
                }
                else
                {
                    twitchUsers =
                        await _twitchManager.GetTwitchUsersByIdsDelimitedList($"&id={string.Join("&id=", guildGroupChannels.Select(ggc => ggc.ChannelId))}");
                }

                if (twitchUsers != null && twitchUsers.Users.Count > 0)
                {
                    builder.Fields.Add(new EmbedFieldBuilder
                    {
                        Name = $"{platform} Creators",
                        Value = string.Join(", ", twitchUsers.Users.Select(u => u.Login)),
                        IsInline = false
                    });
                }
            }       
            else
            {
                builder.Fields.Add(new EmbedFieldBuilder
                    {
                        Name = $"{platform} Creators",
                        Value = "Sorry, you have no creators listed.",
                        IsInline = false
                    }
                );
            }

            await Context.Channel.SendMessageAsync("", false, builder.Build());
        }
    }
}
