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
    public class CreatorCommands : BaseCommand
    {
        private readonly IGroupManager _groupManager;
        private readonly IChannelManager _channelManager;
        private readonly ITwitchManager _twitchManager;
        private readonly IMixerManager _mixerManager;
        private readonly IYouTubeManager _youTubeManager;

        public CreatorCommands(List<Translation> translations, IGuildManager guildManager, IGroupManager groupManager, 
            IConfiguration configuration, IChannelManager channelManager, ITwitchManager twitchManager, IMixerManager mixerManager,
            IYouTubeManager youTubeManager) : base(translations, guildManager, groupManager, configuration)
        {
            _groupManager = groupManager;
            _channelManager = channelManager;
            _twitchManager = twitchManager;
            _mixerManager = mixerManager;
            _youTubeManager = youTubeManager;
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

            var builder = new EmbedBuilder {Description = $"Your list of {platform} creators in the {groupName} group"};

            if (guildGroupChannels != null && guildGroupChannels.Count > 0)
            {
                switch (platform)
                {
                    case Platform.Twitch:
                        var twitchUsers = new TwitchUserQueryResponse();
                        if (guildGroupChannels.Count == 1)
                        {
                            twitchUsers =
                                await _twitchManager.GetTwitchUserById(guildGroupChannels.FirstOrDefault()?.ChannelId);
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
                        break;
                    case Platform.Mixer:
                        var mixerCreatorList = new List<string>();
                        foreach (var creator in guildGroupChannels)
                        {
                            var mixerUser =
                                await _mixerManager.GetMixerChannelByChannelName(creator.ChannelId);

                            if (mixerUser != null)
                            {
                                mixerCreatorList.Add(mixerUser._User.Username);
                            }
                        }

                        builder.Fields.Add(new EmbedFieldBuilder
                        {
                            Name = $"{platform} Creators",
                            Value = string.Join(", ", mixerCreatorList),
                            IsInline = false
                        });
                        break;
                    case Platform.YouTube:
                        var youTubeCreatorList = new List<string>();
                        foreach (var creator in guildGroupChannels)
                        {
                            var youtubeChannel =
                                await _youTubeManager.GetYouTubeChannelByChannelId(creator.ChannelId);

                            if (youtubeChannel != null)
                            {
                                youTubeCreatorList.Add(
                                    $"{youtubeChannel.Items.FirstOrDefault()?.Snippet.Title} ({creator.ChannelId})");
                            }
                        }

                        builder.Fields.Add(new EmbedFieldBuilder
                        {
                            Name = $"{platform} Creators",
                            Value = string.Join(", ", youTubeCreatorList),
                            IsInline = false
                        });
                        break;
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
