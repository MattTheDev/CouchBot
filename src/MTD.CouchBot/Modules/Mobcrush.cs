using Discord.Commands;
using Microsoft.Extensions.Options;
using MTD.CouchBot.Domain;
using MTD.CouchBot.Domain.Models.Bot;
using MTD.CouchBot.Managers;
using MTD.CouchBot.Models.Bot;
using MTD.CouchBot.Services;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MTD.CouchBot.Modules
{
    [Group("mobcrush")]
    public class Mobcrush : BaseModule
    {
        private readonly IMobcrushManager _mobcrushManager;
        private readonly MessagingService _messagingService;
        private readonly BotSettings _botSettings;
        private readonly FileService _fileService;

        public Mobcrush(IMobcrushManager mobcrushManager, MessagingService messagingService, IOptions<BotSettings> botSettings, FileService fileService)
            : base (botSettings)
        {
            _mobcrushManager = mobcrushManager;
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

            var channel = await _mobcrushManager.GetMobcrushIdByName(channelName);

            if (channel == null)
            {
                await Context.Channel.SendMessageAsync("The Mobcrush channel, " + channelName + ", does not exist.");
                return;
            }

            var file = $"{_botSettings.DirectorySettings.ConfigRootDirectory}{_botSettings.DirectorySettings.GuildDirectory}{Context.Guild.Id}.json";
            DiscordServer server = null;

            if (File.Exists(file))
            {
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));
            }

            if (server == null)
            {
                return;
            }

            if (server.ServerHitboxChannels == null)
            {
                server.ServerHitboxChannels = new List<string>();
            }
                        
            if (!string.IsNullOrEmpty(server.OwnerMobcrushId) && server.OwnerMobcrushId.Equals(channel.Channel.Id))
            {
                await Context.Channel.SendMessageAsync("The channel " + channelName + " is configured as the Owner Mobcrush channel. " +
                    "Please remove it with the '!cb mobcrush resetowner' command and then try re-adding it.");

                return;
            }

            if (server.ServerMobcrushIds == null || !server.ServerMobcrushIds.Contains(channel.Channel.Id))
            {
                if(server.ServerMobcrushIds == null)
                {
                    server.ServerMobcrushIds = new List<string>();
                }

                server.ServerMobcrushIds.Add(channel.Channel.Id);
                await _fileService.SaveDiscordServer(server, Context.Guild);
                await Context.Channel.SendMessageAsync("Added " + channelName + " to the server Mobcrush streamer list.");
            }
            else
            {
                await Context.Channel.SendMessageAsync(channelName + " is already on the server Mobcrush streamer list.");
            }
        }

        [Command("remove")]
        public async Task Remove(string channelName)
        {
            if (!IsApprovedAdmin)
            {
                return;
            }

            var file = $"{_botSettings.DirectorySettings.ConfigRootDirectory}{_botSettings.DirectorySettings.GuildDirectory}{Context.Guild.Id}.json";
            DiscordServer server = null;

            if (File.Exists(file))
            {
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));
            }

            if (server == null)
            {
                return;
            }

            if (server.ServerMobcrushIds == null)
            {
                return;
            }

            var channel = await _mobcrushManager.GetMobcrushIdByName(channelName);

            if (channel == null)
            {
                await Context.Channel.SendMessageAsync("The Mobcrush channel, " + channelName + ", does not exist.");
                return;
            }

            if (server.ServerMobcrushIds.Contains(channel.Channel.Id))
            {
                server.ServerMobcrushIds.Remove(channel.Channel.Id);
                await _fileService.SaveDiscordServer(server, Context.Guild);
                await Context.Channel.SendMessageAsync("Removed " + channelName + " from the server Mobcrush streamer list.");
            }
            else
            {
                await Context.Channel.SendMessageAsync(channelName + " wasn't on the server Mobcrush streamer list.");
            }
        }

        [Command("owner")]
        public async Task Owner(string channelName)
        {
            if (!IsAdmin)
            {
                return;
            }

            var channel = await _mobcrushManager.GetMobcrushIdByName(channelName);

            if (channel == null)
            {
                await Context.Channel.SendMessageAsync("The Mobcrush channel, " + channelName + ", does not exist.");
                return;
            }

            var file = $"{_botSettings.DirectorySettings.ConfigRootDirectory}{_botSettings.DirectorySettings.GuildDirectory}{Context.Guild.Id}.json";
            var server = new DiscordServer();

            if (File.Exists(file))
            {
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));
            }

            if (server.ServerMobcrushIds != null && server.ServerMobcrushIds.Contains(channel.Channel.Id))
            {
                await Context.Channel.SendMessageAsync("The channel " + channelName + " is in the list of server Mobcrush Channels. " +
                    "Please remove it with '!cb mobcrush remove " + channelName + "' and then retry setting your owner channel.");

                return;
            }

            server.OwnerMobcrushId = channel.Channel.Id;
            await _fileService.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync("Owner Mobcrush Channel has been set to " + channelName + ".");

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
            {
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));
            }

            server.OwnerMobcrushId = null;
            await _fileService.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync("Owner Mobcrush Channel has been reset.");
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
            {
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));
            }

            var channel = await _mobcrushManager.GetMobcrushIdByName(channelName);

            if (channel == null || channel.Users.Count == 0)
            {
                await Context.Channel.SendMessageAsync(channelName + " doesn't exist on Mobcrush.");

                return;
            }

            var broadcast = await _mobcrushManager.GetMobcrushBroadcastByChannelId(channel.Channel.Id);
            var user = await _mobcrushManager.GetMobcrushStreamById(broadcast.User.Id);

            if (!broadcast.IsLive)
            {
                await Context.Channel.SendMessageAsync(channelName + " isn't currently live on Mobcrush.");

                return;
            }

            if (broadcast.IsLive)
            {
                var gameName = broadcast.Game == null ? "A game" : broadcast.Game.Name;
                var url = "http://mobcrush.com/" + channelName;
                var avatarUrl = user.ProfileLogo == null ? "http://cdn.mobcrush.com/static/images/default-profile-pic.png" : user.ProfileLogo;
                var thumbnailUrl = "http://cdn.mobcrush.com/u/video/" + broadcast.Id + "/snapshot.jpg";

                var message = await _messagingService.BuildMessage(channelName, gameName, "",
                    url, avatarUrl, thumbnailUrl, Constants.Mobcrush, channelName, server, server.GoLiveChannel, null, false, user.ViewCount, null, user.FollowerCount);
                await _messagingService.SendMessages(Constants.Mobcrush, new List<BroadcastMessage> { message });
            }
            else
            {
                await Context.Channel.SendMessageAsync(channelName + " is offline.");
            }
        }
    }
}
