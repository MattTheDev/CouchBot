using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Options;
using MTD.CouchBot.Domain.Enums;
using MTD.CouchBot.Domain.Models.Bot;
using MTD.CouchBot.Services;

namespace MTD.CouchBot.Modules
{
    [Group("DLive")]
    public class DLive : BaseModule
    {
        private readonly FileService _fileService;

        public DLive(IOptions<BotSettings> botSettings, FileService fileService) : base(botSettings, fileService)
        {
            _fileService = fileService;
        }

        [Command("add")]
        [Alias("+")]
        public async Task Add(string channelName, IGuildChannel guildChannel)
        {
            if (!IsAdmin)
            {
                return;
            }

            var server = GetServer();

            if (server == null)
            {
                server = new DiscordServer();
            }

            if (server.Streamers == null)
            {
                server.Streamers = new List<DiscordStreamer>();
            }

            var streamer = server.Streamers.Count == 0 ? null : server.Streamers
                .FirstOrDefault(s => s.Platform == Platform.DLive &&
                                     s.DisordChannelId == guildChannel.Id &&
                                     s.StreamerChannelId == channelName);

            if (streamer != null)
            {
                await Context.Channel.SendMessageAsync("Sorry, that streamer is already configured on that channel.");
                return;
            }

            streamer = new DiscordStreamer
            {
                DisordChannelId = guildChannel.Id,
                Platform = Platform.DLive,
                StreamerChannelId = channelName
            };

            server.Streamers.Add(streamer);

            _fileService.SaveDiscordServer(server);

            await Context.Channel.SendMessageAsync($"{channelName} will now announce on #{guildChannel} when they go live.");
        }

        [Command("remove")]
        [Alias("-")]
        public async Task Remove(string channelName, IGuildChannel guildChannel)
        {
            if (!IsAdmin)
            {
                return;
            }

            await Context.Channel.SendMessageAsync("// TODO");
        }
    }
}