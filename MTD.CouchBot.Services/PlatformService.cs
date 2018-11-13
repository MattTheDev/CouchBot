using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MTD.CouchBot.Domain.Dtos.Discord;
using MTD.CouchBot.Domain.Enums;
using MTD.CouchBot.Domain.Models;
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
            var allChannels = GetCreatorChannelsIdsFromGuildGroupList(allGroups);

            var streamResponse = await _twitchManager.GetTwitchStreamsByUserIdsDelimitedList(allChannels.ToList());

            if (streamResponse == null || streamResponse.Streams.Count == 0)
            {
                return; // No one live. Move along :)
            }

            var streamsToAnnounce = new List<StreamToAnnounce>();

            foreach (var stream in streamResponse.Streams)
            {
                foreach (var group in GetGroupsToAnnounce(allGroups, stream.UserId))
                {
                    var streamToAnnounce = new StreamToAnnounce
                    {
                        GuildGroups = group,
                        Platform = Platform.Twitch,
                        CreatorChannelId = stream.Id,
                        GameId = stream.GameId,
                        TagIds = stream.Tags
                    };

                    streamsToAnnounce.Add(streamToAnnounce);
                }
            }

            var gameIds = streamsToAnnounce.Select(x => x.GameId).Distinct();
            var gameList = await _twitchManager.GetTwitchGamesByIdsDelimitedList(gameIds.ToList());

            foreach (var game in gameList.Games)
            {
                streamsToAnnounce.FirstOrDefault(sta => sta.GameId.Equals(game.Id)).Game = game.Name;
            }

            var message = _messagingService.BuildMessage()


            //var guildId = Cryptography.Decrypt(group.GuildId);
            //var channelId = Cryptography.Decrypt(group.StreamChannelId);
            //var channel = _guildInteractionService.GetChannelById(ulong.Parse(channelId));
            // TODO Get Users for the Avatar, Username, Followers, TotalViews
            // TODO Get Game endpoint
            //var message = await _messagingService.BuildMessage(group.LiveMessage, Platform.Twitch, guildId, channelId,
            //    null, stream.Id, stream.Username, stream.GameId, stream.Title,
            //    $"https://twitch.tv/{stream.Username}", 0, 0, stream.ThumbnailUrl);

            //await channel.SendMessageAsync($" ", false, message.Embed);
        }

        public IEnumerable<string> GetCreatorChannelsIdsFromGuildGroupList(List<GuildGroup> guildGroups)
        {
            var allChannels = new List<string>();

            foreach (var group in guildGroups)
            {
                allChannels.AddRange(group.Channels.Select(x => x.ChannelId));
            }

            return allChannels.Distinct();
        }

        public IEnumerable<GuildGroup> GetGroupsToAnnounce(List<GuildGroup> guildGroups, string userId)
        {
            return guildGroups.Where(ag => ag.Channels.Any(c => c.ChannelId.Equals(userId)));
        }
    }
}