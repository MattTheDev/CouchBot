using Discord.Commands;
using Microsoft.Extensions.Options;
using MTD.CouchBot.Domain;
using MTD.CouchBot.Domain.Models.Bot;
using MTD.CouchBot.Services;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MTD.CouchBot.Managers;

namespace MTD.CouchBot.Modules
{
    [Group("smashcast")]
    public class Smashcast : BaseModule
    {
        private readonly SmashcastManager _smashcastManager;
        private readonly MessagingService _messagingService;
        private readonly BotSettings _botSettings;
        private readonly FileService _fileService;

        public Smashcast(SmashcastManager smashcastManager, MessagingService messagingService, IOptions<BotSettings> botSettings, FileService fileService)
            : base(botSettings, fileService)
        {
            _smashcastManager = smashcastManager;
            _messagingService = messagingService;
            _botSettings = botSettings.Value;
            _fileService = fileService;
        }
        
        [Command("add")]
        public async Task Add(string channelName)
        {
            if (!IsApprovedAdmin)
            {
                return;
            }

            var channel = await _smashcastManager.GetChannelByName(channelName);

            if (channel == null)
            {
                await Context.Channel.SendMessageAsync("The Smashcast channel, " + channelName + ", does not exist.");
                return;
            }

            var file = $"{_botSettings.DirectorySettings.ConfigRootDirectory}{_botSettings.DirectorySettings.GuildDirectory}{Context.Guild.Id}.json";
            DiscordServer server = null;

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            if (server == null)
                return;

            if (server.ServerHitboxChannels == null)
                server.ServerHitboxChannels = new List<string>();
                        
            if (!string.IsNullOrEmpty(server.OwnerHitboxChannel) && server.OwnerHitboxChannel.ToLower().Equals(channelName.ToLower()))
            {
                await Context.Channel.SendMessageAsync("The channel " + channelName + " is configured as the Owner Smashcast channel. " +
                    "Please remove it with the '!cb smashcast resetowner' command and then try re-adding it.");

                return;
            }

            if (!server.ServerHitboxChannels.Contains(channelName.ToLower()))
            {
                server.ServerHitboxChannels.Add(channelName.ToLower());
                await _fileService.SaveDiscordServer(server, Context.Guild);
                await Context.Channel.SendMessageAsync("Added " + channelName + " to the server Smashcast streamer list.");
            }
            else
            {
                await Context.Channel.SendMessageAsync(channelName + " is already on the server Smashcast streamer list.");
            }
        }

        [Command("remove")]
        public async Task Remove(string channel)
        {
            if (!IsApprovedAdmin)
            {
                return;
            }

            var file = $"{_botSettings.DirectorySettings.ConfigRootDirectory}{_botSettings.DirectorySettings.GuildDirectory}{Context.Guild.Id}.json";
            DiscordServer server = null;

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            if (server == null)
                return;

            if (server.ServerHitboxChannels == null)
                return;

            if (server.ServerHitboxChannels.Contains(channel.ToLower()))
            {
                server.ServerHitboxChannels.Remove(channel.ToLower());
                await _fileService.SaveDiscordServer(server, Context.Guild);
                await Context.Channel.SendMessageAsync("Removed " + channel + " from the server Smashcast streamer list.");
            }
            else
            {
                await Context.Channel.SendMessageAsync(channel + " wasn't on the server Smashcast streamer list.");
            }
        }

        [Command("owner")]
        public async Task Owner(string channelName)
        {
            if (!IsAdmin)
            {
                return;
            }

            var channel = await _smashcastManager.GetChannelByName(channelName);

            if (channel == null)
            {
                await Context.Channel.SendMessageAsync("The Smashcast channel, " + channelName + ", does not exist.");
                return;
            }

            var file = $"{_botSettings.DirectorySettings.ConfigRootDirectory}{_botSettings.DirectorySettings.GuildDirectory}{Context.Guild.Id}.json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            if (server.ServerHitboxChannels != null && server.ServerHitboxChannels.Contains(channelName.ToLower()))
            {
                await Context.Channel.SendMessageAsync("The channel " + channelName + " is in the list of server Smashcast Channels. " +
                    "Please remove it with '!cb smashcast remove " + channelName + "' and then retry setting your owner channel.");

                return;
            }

            server.OwnerHitboxChannel = channelName;
            await _fileService.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync("Owner Smashcast Channel has been set to " + channelName + ".");

        }

        [Command("resetowner")]
        public async Task ResetOwner()
        {
            if (!IsAdmin)
            {
                return;
            }

            var file = $"{_botSettings.DirectorySettings.ConfigRootDirectory}{_botSettings.DirectorySettings.GuildDirectory}{Context.Guild.Id}.json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            server.OwnerHitboxChannel = null;
            await _fileService.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync("Owner Smashcast Channel has been reset.");
        }

        [Command("announce")]
        public async Task Announce(string channelName)
        {
            if (!IsAdmin)
            {
                return;
            }

            var file = $"{_botSettings.DirectorySettings.ConfigRootDirectory}{_botSettings.DirectorySettings.GuildDirectory}{Context.Guild.Id}.json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            var stream = await _smashcastManager.GetChannelByName(channelName);

            if (stream == null)
            {
                await Context.Channel.SendMessageAsync(channelName + " doesn't exist on Smashcast.");

                return;
            }

            if (stream.livestream[0].media_is_live == "1")
            {
                var gameName = stream.livestream[0].category_name ?? "A game";
                var url = "http://smashcast.tv/" + channelName;
                var avatarUrl = "http://edge.sf.hitbox.tv" + stream.livestream[0].channel.user_logo;
                var thumbnailUrl = "http://edge.sf.hitbox.tv" + stream.livestream[0].media_thumbnail_large;

                var message = _messagingService.BuildMessage(channelName, gameName, stream.livestream[0].media_status,
                    url, avatarUrl, thumbnailUrl, Constants.Smashcast, channelName, server, server.GoLiveChannel, null, false);
                await _messagingService.SendMessages(Constants.Smashcast, new List<BroadcastMessage>() { message });
            }
            else
            {
                await Context.Channel.SendMessageAsync(channelName + " is offline.");
            }
        }
    }
}
