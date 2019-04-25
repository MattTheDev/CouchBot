using Discord;
using Discord.Commands;
using Microsoft.Extensions.Options;
using MTD.CouchBot.Domain.Models.Bot;
using MTD.CouchBot.Services;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace MTD.CouchBot.Modules
{
    [Group("channel"), Summary("Subset of Commands to configure server channels.")]
    public class Channel : ModuleBase
    {
        private readonly BotSettings _botSettings;
        private readonly FileService _fileService;

        public Channel(IOptions<BotSettings> botSettings, FileService fileService)
        {
            _botSettings = botSettings.Value;
            _fileService = fileService;
        }

        [Command("live"), Summary("Sets go live channel.")]
        public async Task Live(IGuildChannel guildChannel)
        {
            var user = ((IGuildUser)Context.Message.Author);

            if (!user.GuildPermissions.ManageGuild)
            {
                return;
            }

            var file = _botSettings.DirectorySettings.ConfigRootDirectory + _botSettings.DirectorySettings.GuildDirectory + guildChannel.Guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            server.GoLiveChannel = guildChannel.Id;
            await _fileService.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync("The Live Channel has been set.");
        }

        [Command("ownerlive"), Summary("Sets owner live channel.")]
        public async Task OwnerLive(IGuildChannel guildChannel)
        {
            var user = ((IGuildUser)Context.Message.Author);

            if (!user.GuildPermissions.ManageGuild)
            {
                return;
            }

            var file = _botSettings.DirectorySettings.ConfigRootDirectory + _botSettings.DirectorySettings.GuildDirectory + guildChannel.Guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            server.OwnerLiveChannel = guildChannel.Id;
            await _fileService.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync("The Owner Live Channel has been set.");
        }

        [Command("greetings"), Summary("Sets greetings channel.")]
        public async Task Greetings(IGuildChannel guildChannel)
        {
            var user = ((IGuildUser)Context.Message.Author);

            if (!user.GuildPermissions.ManageGuild)
            {
                return;
            }

            var file = _botSettings.DirectorySettings.ConfigRootDirectory + _botSettings.DirectorySettings.GuildDirectory + guildChannel.Guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            server.GreetingsChannel = guildChannel.Id;
            await _fileService.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync("The Greetings Channel has been set.");
        }

        [Command("published"), Summary("Sets published video channel.")]
        public async Task Published(IGuildChannel guildChannel)
        {
            var user = ((IGuildUser)Context.Message.Author);

            if (!user.GuildPermissions.ManageGuild)
            {
                return;
            }

            var file = _botSettings.DirectorySettings.ConfigRootDirectory + _botSettings.DirectorySettings.GuildDirectory + guildChannel.Guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            server.PublishedChannel = guildChannel.Id;
            await _fileService.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync("The Published Channel has been set.");
        }

        [Command("ownerpublished"), Summary("Sets owner published video channel.")]
        public async Task OwnerPublished(IGuildChannel guildChannel)
        {
            var user = ((IGuildUser)Context.Message.Author);

            if (!user.GuildPermissions.ManageGuild)
            {
                return;
            }

            var file = _botSettings.DirectorySettings.ConfigRootDirectory + _botSettings.DirectorySettings.GuildDirectory + guildChannel.Guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            server.OwnerPublishedChannel = guildChannel.Id;
            await _fileService.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync("The Owner Published Channel has been set.");
        }
        
        [Command("twitter")]
        public async Task TwitterChannel(IGuildChannel guildChannel)
        {
            var user = ((IGuildUser)Context.Message.Author);

            if (!user.GuildPermissions.ManageGuild)
            {
                return;
            }

            var server = _fileService.GetConfiguredServerById(Context.Guild.Id);

            if(server == null)
            {
                return;
            }

            server.TwitterChannel = guildChannel.Id;
            await _fileService.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync("The Twitter Channel has been set.");
        }

        [Command("ownertwitter")]
        public async Task OwnerTwitterChannel(IGuildChannel guildChannel)
        {
            var user = ((IGuildUser)Context.Message.Author);

            if (!user.GuildPermissions.ManageGuild)
            {
                return;
            }

            var server = _fileService.GetConfiguredServerById(Context.Guild.Id);

            if (server == null)
            {
                return;
            }

            server.OwnerTwitterChannel = guildChannel.Id;
            await _fileService.SaveDiscordServer(server, Context.Guild);
            await Context.Channel.SendMessageAsync("The Owner Twitter Channel has been set.");
        }

        [Command("clear"), Summary("Clears channels settings for a guild.")]
        public async Task Clear(string option)
        {
            var guild = ((IGuildUser)Context.Message.Author).Guild;
            var user = ((IGuildUser)Context.Message.Author);

            if (!user.GuildPermissions.ManageGuild)
            {
                return;
            }

            var file = _botSettings.DirectorySettings.ConfigRootDirectory + _botSettings.DirectorySettings.GuildDirectory + guild.Id + ".json";
            var server = new DiscordServer();

            if (File.Exists(file))
                server = JsonConvert.DeserializeObject<DiscordServer>(File.ReadAllText(file));

            if (File.Exists(file))
            {
                option = option.ToLower();
                var label = "";

                switch (option)
                {
                    case "live":
                        server.GoLiveChannel = 0;
                        label = "Live Channel";
                        break;
                    case "ownerlive":
                        server.OwnerLiveChannel = 0;
                        label = "Owner Live Channel";
                        break;
                    case "greetings":
                        server.GreetingsChannel = 0;
                        label = "Greetings";
                        break;
                    case "ownerpublished":
                        server.OwnerPublishedChannel = 0;
                        label = "Owner Published";
                        break;
                    case "published":
                        server.PublishedChannel = 0;
                        label = "Published";
                        break;
                    case "twitter":
                        server.TwitterChannel = 0;
                        label = "Twitter";
                        break;
                    case "ownertwitter":
                        server.OwnerTwitterChannel = 0;
                        label = "Owner Twitter";
                        break;
                    case "all":
                        server.GoLiveChannel = 0;
                        server.GreetingsChannel = 0;
                        server.PublishedChannel = 0;
                        server.OwnerPublishedChannel = 0;
                        server.OwnerLiveChannel = 0;
                        server.TwitterChannel = 0;
                        server.OwnerTwitterChannel = 0;
                        label = "All";
                        break;
                }

                if (!string.IsNullOrEmpty(label))
                {
                    await _fileService.SaveDiscordServer(server, Context.Guild);
                    await Context.Channel.SendMessageAsync(label + " settings have been reset.");
                }
            }
        }
    }
}
