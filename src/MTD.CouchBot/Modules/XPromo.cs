using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Options;
using MTD.CouchBot.Domain.Enums;
using MTD.CouchBot.Domain.Models.Bot;
using MTD.CouchBot.Domain.Utilities;
using MTD.CouchBot.Services;

namespace MTD.CouchBot.Modules
{
    [Group("XPromo")]
    public class XPromo : BaseModule
    {
        private readonly FileService _fileService;

        public XPromo(IOptions<BotSettings> botSettings, FileService fileService)
            : base(botSettings, fileService)
        {
            _fileService = fileService;
        }

        [Command("Allow")]
        public async Task Allow(Platform platform, string trueFalse)
        {
            if (!IsAdmin)
            {
                return;
            }

            if (!trueFalse.IsBoolean())
            {
                await Context.Channel.SendMessageAsync("Pass true or false when configuring Allow for XPromo. (ie: !cb xpromo youtube true)");
                return;
            }

            var enabled = bool.Parse(trueFalse);

            var server = GetServer();

            if (server.XPromo == null)
            {
                server.XPromo = new Domain.Models.Bot.XPromo();
            }

            switch (platform)
            {
                case Platform.Mixer:
                    server.XPromo.AllowMixer = enabled;
                    break;
                case Platform.Mobcrush:
                    server.XPromo.AllowMobcrush = enabled;
                    break;
                case Platform.Picarto:
                    server.XPromo.AllowPicarto = enabled;
                    break;
                case Platform.Piczel:
                    server.XPromo.AllowPiczel = enabled;
                    break;
                case Platform.Smashcast:
                    server.XPromo.AllowSmashcast = enabled;
                    break;
                case Platform.Twitch:
                    server.XPromo.AllowTwitch = enabled;
                    break;
                case Platform.YouTube:
                    server.XPromo.AllowYouTube = enabled;
                    break;
                case Platform.All:
                    server.XPromo.AllowMixer = enabled;
                    server.XPromo.AllowMobcrush = enabled;
                    server.XPromo.AllowPicarto = enabled;
                    server.XPromo.AllowPiczel = enabled;
                    server.XPromo.AllowSmashcast = enabled;
                    server.XPromo.AllowTwitch = enabled;
                    server.XPromo.AllowYouTube = enabled;
                    break;
            }

            await Context.Channel.SendMessageAsync($"You've successfully set XPromo to {enabled} " +
                                                   $"on {platform}.");
            _fileService.SaveDiscordServer(server);
        }

        [Command("Channel")]
        public async Task Channel(IGuildChannel channel)
        {
            if (!IsAdmin)
            {
                return;
            }

            var server = GetServer();

            if (server.XPromo == null)
            {
                server.XPromo = new Domain.Models.Bot.XPromo();
            }

            server.XPromo.ChannelId = channel.Id;
            _fileService.SaveDiscordServer(server);

            await Context.Channel.SendMessageAsync($"Your XPromo channel has been set to {channel.Name}.");
        }
    }
}