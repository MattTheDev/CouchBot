using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MTD.CouchBot.Domain.Dtos.Discord;
using MTD.CouchBot.Domain.Enums;
using MTD.CouchBot.Domain.Utilities;
using MTD.CouchBot.Managers;

namespace MTD.CouchBot.Services
{
    public class PlatformService
    {
        private readonly ITwitchManager _twitchManager;
        private readonly IGroupManager _groupManager;
        private readonly GuildInteractionService _guildInteractionService;
        private readonly MessagingService _messagingService;

        public PlatformService(IGroupManager groupManager, ITwitchManager twitchManager, GuildInteractionService guildInteractionService, MessagingService messagingService)
        {
            _groupManager = groupManager;
            _twitchManager = twitchManager;
            _guildInteractionService = guildInteractionService;
            _messagingService = messagingService;
        }

        public async Task CheckTwitchStreams()
        {
            var allGroups = await _groupManager.GetAllGuildGroupsWithGroupChannels();
            // TODO Refactor this logic
            var allChannels = new List<string>();

            foreach (var group in allGroups)
            {
                allChannels.AddRange(group.Channels.Select(x => x.ChannelId));
            }

            var distinctChannels = allChannels.Distinct().ToList();

            var streamResponse = await _twitchManager.GetTwitchStreamsByUserIdsDelimitedList(distinctChannels);

            if (streamResponse == null)
            {
                return;
            }

            foreach (var stream in streamResponse.Streams)
            {
                var groupsToAnnounce = allGroups.Where(ag => ag.Channels.Any(c => c.ChannelId.Equals(stream.UserId)));

                foreach (var group in groupsToAnnounce)
                {
                    var guildId = Cryptography.Decrypt(group.GuildId);
                    var channelId = Cryptography.Decrypt(group.StreamChannelId);
                    var channel = _guildInteractionService.GetChannelById(ulong.Parse(channelId));

                    // TODO Get Users for the Avatar, Username, Followers, TotalViews
                    // TODO Get Game endpoint
                    var message = await _messagingService.BuildMessage(group.LiveMessage, Platform.Twitch, guildId, channelId,
                        null, stream.Id, stream.Username, stream.GameId, stream.Title,
                        $"https://twitch.tv/{stream.Username}", 0, 0, stream.ThumbnailUrl);

                    await channel.SendMessageAsync($" ", false, message.Embed);
                }
            }
        }
    }
}